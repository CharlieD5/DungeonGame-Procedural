using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBoardGenerator : MonoBehaviour
{
    #region Public Variables

    [Header("Parameters")]

    // ability to regenerate the same map over and over
    public int seed = 0;
    // how far away victory room is from start room (by room)
    public int victoryDepth = 8;
    // max number of tries
    public int maxTries = 100;

    // list of text assets to load texts from text files
    public List<TextAsset> roomAssets;

    [System.Serializable]
    public struct TileData
    {
        public char tileType;
        public Tile prefab;
    }

    // chars like " ", -, |, N, S, E, W, ...
    public List<TileData> tileDatas;

    [Header("UI & Textures")]

    public Text seedText;
    public Material m_VictoryFloor, m_VictoryWall;

    [Header("Etc.")]

    // current board we are building
    public TileBoard board;
    public GameObject replaceDoor;

    #endregion

    #region Private Variables

    private Dictionary<char, List<string>> sortedDoorRooms = new Dictionary<char, List<string>>();
    private List<char> dirs = new List<char> { 'N', 'E', 'S', 'W' };

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        SortRoomsViaExits();
        // probably should be called on loading screen rather than start
        Generate();
    }

    public void Generate()
    {
        Random.InitState(seed);
        seedText.text = "Seed: " + seed.ToString();
        // create new tile board
        GameObject tileObj = new GameObject("Tile Board");
        board = tileObj.AddComponent<TileBoard>();

        // create a starting room
        int roomIndex = 0;
        string strRoom = roomAssets[roomIndex].text;    // start room assigned
        Room startRoom = CreateRoomFromString(strRoom, Vector2Int.zero);

        // for each unconnected door in the start room, recursively create a new room

        GenerateConnectedRooms(startRoom, 0);
        RemoveUnconnectedDoors();
    }


    private Room CreateRoomFromString(string strRoom, Vector2Int roomOriginBoardPos)
    {
        // create a new room
        GameObject roomObj = new GameObject($"Room {board.rooms.Count}");
        roomObj.transform.SetParent(board.transform);
        Room room = roomObj.AddComponent<Room>();
        room.strRoom = strRoom;
        board.rooms.Add(room);

        // iterate through each character in room string and spawn a tile matching that type
        Vector2Int roomPos = Vector2Int.zero;
        for (int i = 0; i < strRoom.Length; i++)
        {
            char tileType = strRoom[i];

            if (tileType == '\n')
            {
                roomPos.x = 0;
                roomPos.y++;
                continue;
            }

            if (tileType == '\r')
            {
                continue;
            }

            SpawnTile(tileType, room, roomPos + roomOriginBoardPos);

            roomPos.x++;
        }

        return room;
    }

    private float GetTileRotation(char tileType)
    {
        float rot = 0;
        switch (tileType)
        {
            case 'N':
            case 'S':
                rot = 90;
                break;
        }
        return rot;
    }


    // tile type, type of room, room position to spawn it in
    private Tile SpawnTile(char tileType, Room room, Vector2Int roomPos)
    {
        // check if a TileData matches that tile type
        TileData tileData = tileDatas.Find(td => td.tileType == tileType);
        if (tileData.prefab == null)
            return null;

        Tile tile = Instantiate(tileData.prefab, room.transform);
        tile.transform.localPosition = new Vector3(roomPos.x, 0, roomPos.y);
        tile.roomPosition = new Vector2Int(Mathf.FloorToInt(tile.transform.position.x), Mathf.FloorToInt(tile.transform.position.z));
        float yRot = GetTileRotation(tileType);
        tile.transform.rotation = Quaternion.Euler(0, yRot, 0);

        tile.tileType = tileType;
        tile.room = room;
        room.tiles.Add(tile);

        // does tile have door in it? If it does then add door to that tile
        Door door = tile.GetComponent<Door>();
        if (door != null)
        {
            room.doors.Add(door);
            door.tile = tile;
            door.direction = tileType;
        }

        return tile;
    }

    // what room and depth
    private void GenerateConnectedRooms(Room room, int depth)
    {
        if (depth == 9 || depth == 8)
        {
            int i = 0;
        }
        List<Room> roomList = new List<Room>();

        //list of added rooms
        foreach (Door door in room.doors)
        {
            char matchingDir = matchingDoorDirection(door.direction);

            List<string> possibleRoomsList = new List<string>(sortedDoorRooms[matchingDir]);

            for (int i = 0; i < maxTries; i++)
            {
                int randIndex = Random.Range(0, possibleRoomsList.Count);
                string newRoom = possibleRoomsList[randIndex];


                Vector2Int originPos = GetOriginPosition(matchingDir, newRoom);
                Vector2Int newPos = door.tile.roomPosition + originPos;
                Vector3 newPos3 = new Vector3(newPos.x, 0, newPos.y);

                if (CheckIfFits(newRoom, newPos3))
                {
                    if (matchingDir == 'E' || matchingDir == 'N' || matchingDir == 'S')
                    {
                        newPos += new Vector2Int(1, 0);
                    }

                    Room AddedRoom = CreateRoomFromString(newRoom, newPos);

                    // find the connected door
                    Door mConnectedDoor = FindConnectedDoor(matchingDir, AddedRoom);

                    // set connected door variables
                    door.connectedDoor = mConnectedDoor;
                    mConnectedDoor.connectedDoor = door;

                    roomList.Add(AddedRoom);

                    break;
                }  
                else
                {
                    possibleRoomsList.Remove(newRoom);
                    if (possibleRoomsList.Count == 0)
                    {
                        break;
                    }
                }
            }
        }

        //added rooms
        if (depth + 1 < victoryDepth)
        {
            foreach (Room addedRoom in roomList)
            {
                GenerateConnectedRooms(addedRoom, depth + 1);
            }
        }
        // TO DO: else, set victory room to look like victory room
        else
        {
            foreach (Room addedRoom in roomList)
            {
                foreach (Tile tile in addedRoom.tiles)
                {
                    if (tile.tileType == ' ')
                    {
                        tile.transform.GetChild(0).GetComponent<Renderer>().material = m_VictoryFloor;
                    }
                }
            }
        }
    }

    private Door FindConnectedDoor(char direction, Room room)
    {
        foreach (Door door in room.doors)
        {
            if (door.direction == direction)
            {
                return door;
            }
        }
        return null;
    }

    // goes through and replaces doors in rooms without a connection to a wall
    private void RemoveUnconnectedDoors()
    {
        foreach (Room room in board.rooms)
        {
            foreach (Door door in room.doors)
            {
                if (door.connectedDoor == null)
                {
                    Instantiate(replaceDoor, door.transform.position, door.transform.rotation, door.transform.parent);
                    Destroy(door.gameObject);
                }
            }
        }

    }

    private void SortRoomsViaExits()
    {
        foreach (char direction in dirs)
        {
            List<string> tempList = new List<string>();
            foreach (UnityEngine.TextAsset item in roomAssets)
            {
                if (item.text.Contains(direction.ToString()))
                {
                    tempList.Add(item.text);
                }
            }
            sortedDoorRooms.Add(direction, tempList);
        }
    }

    private bool CheckIfFits(string strRoom, Vector3 originPos)
    {
        Vector2Int roomPos = Vector2Int.zero;
        for (int i = 0; i < strRoom.Length; i++)
        {
            char tileType = strRoom[i];

            if (tileType == '\n')
            {
                roomPos.x = 0;
                roomPos.y++;
                continue;
            }

            if (tileType == '\r')
            {
                continue;
            }

            roomPos.x++;

            Vector3 roomPos3 = new Vector3(roomPos.x, 0, roomPos.y) + originPos;

            foreach (Room room in board.rooms)
            {
                foreach (Tile tile in room.tiles)
                {
                    if (tile.transform.position == roomPos3)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private Vector2Int GetOriginPosition(char direction, string room)
    {
        Vector2Int originPos = new Vector2Int();
        string[] arr = room.Split('\n');
        switch (direction)
        {
            case 'N':
                originPos = new Vector2Int(-FindDoorLocation(arr, direction) - 1, 1);
                break;

            case 'E':
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].Contains(direction.ToString()))
                    {
                        originPos = new Vector2Int(-arr[i].Length, -FindDoorLocation(arr, direction));
                        break;
                    }
                }
                break;

            case 'S':
                originPos = new Vector2Int(-FindDoorLocation(arr, direction) - 1, -arr.Length);
                break;

            case 'W':
                originPos = new Vector2Int(1, -FindDoorLocation(arr, direction));
                break;
        }
        return originPos;
    }

  
    private int FindDoorLocation(string[] strArr, char direction)
    {
        for (int i = 0; i < strArr.Length; i++)
        {
            if (strArr[i].Contains(direction.ToString()))
            {
                if (direction == 'N' || direction == 'S')
                {
                    return strArr[i].IndexOf(direction);
                }
                else
                {
                    return i;
                }
            }
        }

        return 0;
    }

    private char matchingDoorDirection(char direction)
    {
        if (direction == 'S')
        {
            return 'N';
        }
        else if (direction == 'N')
        {
            return 'S';
        }
        else if (direction == 'W')
        {
            return 'E';
        }
        else
        {
            return 'W';
        }
    }
}
