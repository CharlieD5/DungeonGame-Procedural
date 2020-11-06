using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitDir { North, East, South, West };
public enum Turn { PLAYER, ENEMY, NONE };

public class GameController : MonoBehaviour
{
    public TileBoardGenerator boardGenerator;
    public TileBoard board;

    public PlayerUnit player;
    public GameObject warriorPrefab;
    public GameObject archerPrefab;
    public GameObject spearPrefab;
    public GameObject arrowPrefab;

    public List<Upgrade> upgradesList;

    public List<Unit> curRoomUnits = new List<Unit>();

    public Camera cam;

    public Room curRoom;

    [Header("Materials")]
    public Material tileHighlightMaterial;
    public Tile highlightedTile = null;
    public Material highlightedTilePrevMaterial;
    public Material upgradeMaterial;
    public Material victoryMaterial;
    public Material lavaMaterial;

    [Header("UI")]
    public GameObject playerOptions;
    public GameObject upgradeMachine;
    public Text upgrade1;
    public Text upgrade2;
    public Text upgrade3;

    public Text currentDepthText;
    public int currentDepth = 0;

    public Text currentHealthText;
    public Text currentManaText;

    public Text gameOverText;

    public bool isPlayerDead = false;
    public bool isInLineOfSight = false;

    public Turn turn;

    private bool IsPlayerChoosingUpgrade = false;

    private bool waitForBoradGen = false;

    private static Color playerColor;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        turn = Turn.PLAYER;
        gameOverText.text = "";
        currentDepthText.text = "Depth: " + currentDepth.ToString();
        currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
        currentManaText.text = "Mana: " + player.mana.ToString() + "/" + player.maxMana.ToString();
        upgradeMachine.SetActive(false);
        board = boardGenerator.Generate();
        waitForBoradGen = true;
        Room startRoom = board.rooms[0];
        MoveToRoom(startRoom);
        GameObject spear = Instantiate(spearPrefab) as GameObject;
        spear.transform.parent = player.gameObject.transform;
        player.pickup = spear.GetComponent<Spear>();
        playerColor = player.transform.GetChild(0).GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        if (waitForBoradGen)
        {
            // Player turn
            if (turn == Turn.PLAYER && player.health > 0)
            {
                playerOptions.SetActive(true);

                bool turnTaken = player.GetComponent<PlayerController>().PerformTurn(this);

                if (Input.GetKeyDown(KeyCode.Alpha1))
                    CheatCodeOne();

                if (Input.GetKeyDown(KeyCode.Alpha2))
                    CheatCodeTwo(player.tile.room);

                if (turnTaken)
                    StartCoroutine(SwitchTurns(player, 0.25f));
            }
            else if (turn == Turn.ENEMY)
            {
                playerOptions.SetActive(false);

                // Enemy turn
                foreach (Unit unit in player.tile.room.units)
                {
                    if (unit == null)
                        continue;

                    if (unit.health > 0 && !unit.wasPushed)
                        //StartCoroutine(DelayEnemyTurns(unit, 0.5f));
                        StartCoroutine(SwitchTurns(unit, 0.25f));

                    unit.wasPushed = false;
                }
                turn = Turn.PLAYER;
            }

            if (player.health <= 0 && isPlayerDead != true)
            {
                GameOver(false);
                //StartCoroutine(Death(player, 0.5f));
                StartCoroutine(DelayEnd(1));
               // turn = Turn.;
                isPlayerDead = true;
            }

        }
    }

    #region Move

    void MoveToRoom(Room room, Door entryDoor = null)
    {
        Tile startTile = entryDoor != null ? entryDoor.tile : room.tiles.Find(t => t.IsFloorTile);

        MoveUnitToTile(player, startTile, false);

        FocusCameraOnRoom(room);

        curRoom = room;

        if (curRoom.units == null)
        {
            Spawn(curRoom);
        }

        curRoomUnits = curRoom.units;
    }

    void FocusCameraOnRoom(Room room)
    {
        Vector2Int min = room.tiles[0].BoardPosition;
        Vector2Int max = min;

        foreach (Tile tile in room.tiles)
        {
            Vector2Int roomPos = tile.roomPosition;
            if (roomPos.x < min.x)
                min.x = roomPos.x;
            else if (roomPos.x > max.x)
                max.x = roomPos.x;
            if (roomPos.y < min.y)
                min.y = roomPos.y;
            else if (roomPos.y > max.y)
                max.y = roomPos.y;
        }


        Vector2Int center = (max - min) / 2 + min;
        Vector3 camPos = board.BoardToWorld(center) + Vector3.up * 10f;

        // cam.transform.position = camPos;
        StartCoroutine(MoveTo(cam.transform, camPos, 0.25f));
    }

    public bool CanMoveTo(Unit unit, Tile tile)
    {
        if (tile == null || !(tile.IsFloorTile || tile.IsDoorTile) || tile.unit != null || tile == tile.IsLavaTile)
            return false;

        if (unit.tile != null && !unit.tile.IsAdjacentTo(tile))
            return false;

        if (IsPlayerChoosingUpgrade)
            return false;

        return true;
    }

    public void MoveUnitToTile(Unit unit, Tile target, bool entering = true)
    {
        Tile from = null;

        if (unit.tile != null)
        {
            from = unit.tile;
            unit.tile.unit = null;
        }

        unit.tile = target;
        target.unit = unit;

        StartCoroutine(MoveTo(unit.transform, target.transform.position, 0.25f));

        if (entering)
        {
            OnUnitEnteredTile(unit, target, from);
        }
    }

    void OnUnitEnteredTile(Unit unit, Tile to, Tile from)
    {
        if (to.IsDoorTile)
        {
            Door door = to.GetComponent<Door>();
            Room nextRoom = door.connectedDoor.tile.room;
            currentDepth = nextRoom.depth;
            currentDepthText.text = "Depth: " + currentDepth.ToString();

            MoveToRoom(nextRoom, door.connectedDoor);
            player.mana = player.maxMana;
            currentManaText.text = "Mana: " + player.mana.ToString() + "/" + player.maxMana.ToString();
        }

        if (to.IsUpgradeTile)
        {
            List<Upgrade> upgrades = new List<Upgrade>();
            int i = 0;
            foreach (Upgrade item in upgradesList)
            {
                upgrades.Add(upgradesList[i]);
                i++;
            }

            int r = Random.Range(0, 3);
            upgrades.Remove(upgrades[r]);

            DisplayUpgradeMachine(upgrades);
            currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
            to.IsUpgradeTile = false;
            to.GetComponentInChildren<Renderer>().material = highlightedTilePrevMaterial;
        }

        if (to.IsVictoryTile)
        {
            GameOver(true);
        }

        if (unit.tile.pickup != null && unit.GetComponent<PlayerUnit>())
        {
            player.pickup = unit.tile.pickup;
            unit.tile.pickup = null;
            player.pickup.transform.parent = player.gameObject.transform;
            player.pickup.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void HighlightTile(Tile tile)
    {
        if (tile == highlightedTile || tile == null)
            return;

        if (highlightedTile != null)
        {
            Renderer renderer = highlightedTile.GetComponentInChildren<Renderer>();
            renderer.material = highlightedTilePrevMaterial;
        }

        if (!tile.IsFloorTile)
            return;

        highlightedTile = tile;
        if (highlightedTile != null)
        {
            Renderer renderer = highlightedTile.GetComponentInChildren<Renderer>();
            highlightedTilePrevMaterial = renderer.material;
            renderer.material = tileHighlightMaterial;
        }
    }

    IEnumerator MoveTo(Transform target, Vector3 to, float duration)
    {
        float time = 0;
        Vector3 start = target.position;

        while (time <= duration)
        {
            // TODO: move obj
            float t = Mathf.Clamp01(time / duration);
            target.position = Vector3.Lerp(start, to, t);

            yield return new WaitForEndOfFrame(); //WaitForSeconds(time) will wait for that time
            time += Time.deltaTime;
        }

        target.position = to;
    }

    #endregion

    #region Jump

    public bool CanJumpTo(Unit unit, Tile tile)
    {
        if (tile == null || !(tile.IsFloorTile || tile.IsDoorTile || tile.IsUpgradeTile) || tile.unit != null || tile == tile.IsLavaTile)
            return false;

        if (IsPlayerChoosingUpgrade)
            return false;

        if (unit.tile != null && !unit.tile.IsJumpableTo(tile, player))
            return false;

        if (player.mana < 2)
            return false;

        return true;
    }

    public void JumpUnitToTile(Unit unit, Tile target, bool entering = true)
    {
        Tile from = null;

        if (unit.tile != null)
        {
            from = unit.tile;
            unit.tile.unit = null;
        }

        unit.tile = target;
        target.unit = unit;

        StartCoroutine(JumpTo(player.transform, target.transform.position, 0.25f));
        player.mana = player.mana - 2;
        currentManaText.text = "Mana: " + player.mana.ToString() + "/" + player.maxMana.ToString();

        if (target.IsUpgradeTile)
        {
            List<Upgrade> upgrades = new List<Upgrade>();
            int i = 0;
            foreach (Upgrade item in upgradesList)
            {
                upgrades.Add(upgradesList[i]);
                i++;
            }

            int r = Random.Range(0, 3);
            upgrades.Remove(upgrades[r]);

            DisplayUpgradeMachine(upgrades);
            currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
            target.IsUpgradeTile = false;
            target.GetComponentInChildren<Renderer>().material = highlightedTilePrevMaterial;
        }

        if (entering)
        {
            OnUnitEnteredTile(unit, target, from);
        }
    }

    IEnumerator JumpTo(Transform target, Vector3 to, float duration)
    {
        float time = 0;
        float jumpVal = 2;
        Vector3 start = target.position;

        while (time <= duration)
        {
            float t = Mathf.Clamp01(time / duration);
            float yPos;

            if (t <= 0.40f)
            {
                yPos = Mathf.Lerp(0, jumpVal, t * 2.5f);
            }
            else if (t > 0.40f && t <= 0.60f)
            {
                yPos = jumpVal;
            }
            else
            {
                yPos = Mathf.Lerp(jumpVal, 0, (t - 0.6f) * 2.5f);
            }

            target.position = Vector3.Lerp(start, to, t) + new Vector3(0, yPos, 0);

            yield return new WaitForEndOfFrame(); //WaitForSeconds(time) will wait for that time
            time += Time.deltaTime;
        }

        target.position = to;
    }

    #endregion

    #region Spawn Enemies, Upgrades

    public void Spawn(Room room)
    {
        List<Tile> validFloorTiles = room.tiles.FindAll((Tile T) => T.IsFloorTile && T.unit == null);

        room.units = new List<Unit>();

        // Enemy spawn
        for (int i = 0; i < room.depth + 1; i++)
        {
            int r1 = Random.Range(0, validFloorTiles.Count);
            Tile atTile = validFloorTiles[r1];

            validFloorTiles.RemoveAt(r1);

            int r2 = Random.Range(0, 10);
            if (r2 % 2 == 0)
            {
                room.units.Add(spawnWarrior(atTile));
            }
            else
            {
                room.units.Add(spawnArcher(atTile));
            }
        }

        // Lava tile spawn
        int randNum = Random.Range(1, 4);
        for (int i = 0; i < randNum; i++)
        {
            if (validFloorTiles.Count < 5)
            {
                break;
            }

            int rand = Random.Range(0, validFloorTiles.Count);
            Tile atTile = validFloorTiles[rand];
            atTile.tileType = 'L';
            Renderer rend = atTile.GetComponentInChildren<Renderer>();
            rend.material = lavaMaterial;
            validFloorTiles.RemoveAt(rand);
        }

        // Upgrade tile spawn
        int r3 = Random.Range(0, validFloorTiles.Count);
        Tile tile = validFloorTiles[r3];
        tile.IsUpgradeTile = true;
        Renderer renderer = tile.GetComponentInChildren<Renderer>();
        renderer.material = upgradeMaterial;
        validFloorTiles.RemoveAt(r3);



        // Victory tile spawn
        if (room.depth == boardGenerator.victoryDepth)
        {
            int r4 = Random.Range(0, validFloorTiles.Count);
            Tile t = validFloorTiles[r4];
            t.IsVictoryTile = true;
            Renderer r = t.GetComponentInChildren<Renderer>();
            r.material = victoryMaterial;
        }
    }

    public Unit spawnWarrior(Tile atTile)
    {
        GameObject w = Instantiate(warriorPrefab) as GameObject;
        WarriorUnit warrior = w.GetComponent<WarriorUnit>();
        warrior.transform.position = atTile.transform.position;
        warrior.tile = atTile;
        atTile.unit = warrior;
        return warrior;
    }

    public Unit spawnArcher(Tile atTile)
    {
        GameObject a = Instantiate(archerPrefab) as GameObject;
        ArcherUnit archer = a.GetComponent<ArcherUnit>();
        archer.transform.position = atTile.transform.position;
        archer.tile = atTile;
        atTile.unit = archer;
        return archer;
    }

    public void spawnAndShootArrow(Tile atTile)
    {
        GameObject arrow = Instantiate(arrowPrefab) as GameObject;
        arrow.transform.position = atTile.transform.position;

        // shoots arrow
        if (player != null)
        {
            StartCoroutine(ShootItemTo(arrow.transform, player.transform.position, .2f));
            StartCoroutine(DestroyObject(arrow, .5f));
            arrow = null;
        }

    }

    void DisplayUpgradeMachine(List<Upgrade> upgrades)
    {
        IsPlayerChoosingUpgrade = true;
        upgradeMachine.SetActive(true);
        upgrade1.text = "Press 'S': " + upgrades[0].ToString();
        upgrade2.text = "Press 'D': " + upgrades[1].ToString();
        upgrade3.text = "Press 'F': " + upgrades[2].ToString();
        StartCoroutine(ChooseUpgrade(upgrades));
    }

    public void ThrowSpear(Transform target, Tile atTile)
    {
        StartCoroutine(ShootItemTo(target, atTile.gameObject.transform.position, .3f));
    }

    IEnumerator ChooseUpgrade(List<Upgrade> upgrades)
    {
        do
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                upgrades[0].PerformUpgrade(player);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                upgrades[1].PerformUpgrade(player);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                upgrades[2].PerformUpgrade(player);
                break;
            }

            yield return null;
        } while (true);
        upgradeMachine.SetActive(false);
        IsPlayerChoosingUpgrade = false;
        currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
        currentManaText.text = "Mana: " + player.mana.ToString() + "/" + player.maxMana.ToString();

    }

    IEnumerator ShootItemTo(Transform target, Vector3 to, float duration)
    {
        float time = 0;
        float jumpVal = 1;
        Vector3 start = target.position;
        if (target.position.x != to.x)
        {
            target.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        while (time <= duration)
        {
            float t = Mathf.Clamp01(time / duration);
            float yPos;

            if (t <= 0.50f)
            {
                yPos = Mathf.Lerp(0, jumpVal, t * 2.0f);
            }
            else
            {
                yPos = Mathf.Lerp(jumpVal, 0, (t - 0.5f) * 2.0f);
            }

            target.position = Vector3.Lerp(start, to, t) + new Vector3(0, yPos, 0);

            yield return new WaitForEndOfFrame(); //WaitForSeconds(time) will wait for that time
            time += Time.deltaTime;
        }

        target.position = to;
    }

    #endregion

    #region Cheats

    void CheatCodeOne()
    {
        player.mana = player.maxMana;
        currentManaText.text = "Mana: " + player.mana.ToString() + "/" + player.maxMana.ToString();
        player.health = player.maxHealth;
        currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
    }

    void CheatCodeTwo(Room room)
    {
        foreach (Unit unit in room.units)
        {
            if (unit == null)
                continue;
            unit.health = 0;
            Destroy(unit.gameObject);
        }
        room.units.Clear();
    }

    #endregion

    #region Damage & Death

    IEnumerator DamageAnimation(Unit unit, float delay)
    {
        if (unit)
        {
            Renderer r = unit.transform.GetChild(0).GetComponent<Renderer>();
            Color prevColor = r.material.color;
            r.material.color = new Color(255, 0, 0);
            yield return new WaitForSeconds(delay);
            if (unit is PlayerUnit)
                r.material.color = playerColor;
            else
                r.material.color = prevColor;
        }
    }

    IEnumerator Death(Unit unit, float delay)
    {
        if (unit != null)
        {
            yield return new WaitForSeconds(delay);
            unit.tile.unit = null;
            Destroy(unit.gameObject);
            unit = null;
        }
    }

    IEnumerator DestroyObject(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
        obj = null;
    }

    public void Damage(Unit unit)
    {
        if (unit)
        {
            if (!unit.tile.IsDoorTile)
            {
                unit.health--;
                StartCoroutine(DamageAnimation(unit, .25f));

                if (unit.health <= 0)
                {
                    StartCoroutine(Death(unit, .35f));
                }
                if (unit is PlayerUnit && unit != null)
                    currentHealthText.text = "HP: " + player.health.ToString() + "/" + player.maxHealth.ToString();
            }
        }
    }

    #endregion

    #region GameOver State

    void GameOver(bool isWin)
    {
        if (isWin)
        {
            gameOverText.text = "You Win!!!";
            gameOverText.GetComponent<Text>().color = Color.green;
            StartCoroutine(DelayEnd(1));
        }
        else
        {
            gameOverText.text = "Game Over. You Lose.";
            gameOverText.GetComponent<Text>().color = Color.red;
        }
    }

    IEnumerator DelayEnd(float delay)
    {
        yield return new WaitForSeconds(delay);

        Time.timeScale = 0;
    }

    IEnumerator DelayEnemyTurns(Unit unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        unit.GetComponent<UnitController>().PerformTurn(this);
    }

    IEnumerator SwitchTurns(Unit unit, float delay)
    {
        turn = Turn.NONE;
        if (unit is PlayerUnit)
        {
            yield return new WaitForSeconds(delay);
            unit.GetComponent<UnitController>().PerformTurn(this);
            turn = Turn.ENEMY;
        }
        else
        {
            yield return new WaitForSeconds(delay);
            unit.GetComponent<UnitController>().PerformTurn(this);
        }
    }

    #endregion
}
