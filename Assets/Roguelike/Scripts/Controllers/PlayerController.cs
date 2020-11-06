using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : UnitController
{

    private PlayerUnit player;

    private void Start()
    {
        player = GetComponent<PlayerUnit>();
    }
    
    public override bool PerformTurn(GameController gController)
    {
        bool tookAction = false;
        Ray ray = gController.cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            Vector3 worldPos = ray.GetPoint(dist);
            Vector2Int boardPos = gController.board.WorldToBoard(worldPos);
            Tile tile = gController.board.GetTileAt(boardPos);
            gController.HighlightTile(tile);

            foreach (Action action in player.actions)
            {
                tookAction = action.Perform(gController, player, tile);
                if (tookAction)
                    return true;
            }

         //   if (gController.isSpearThrow)
          //  {
          //      player.spear.pare
           // }
        }
       
        return false;
    }
}
