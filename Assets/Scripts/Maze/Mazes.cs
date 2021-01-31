using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mazes
{

    public const int xsize = 6;
    public const int ysize = 10;
    public const int tilesize = 150;

    static bool t = true;
    static bool f = false;
    public static bool[,,] mazes = new bool[6,ysize,xsize] {
        {{t,f,f,t,t,t},
         {f,f,f,f,t,f},
         {f,t,f,f,t,f},
         {f,t,t,f,t,f},
         {f,t,f,f,t,t},
         {t,t,f,f,f,t},
         {f,t,t,t,f,t},
         {f,t,f,t,f,t},
         {f,t,f,t,t,t},
         {t,t,f,f,t,f}},

        {{f,t,t,f,t,t},
         {f,t,f,t,t,f},
         {f,t,f,t,f,f},
         {f,t,f,f,t,t},
         {t,t,t,t,f,t},
         {t,f,f,t,f,t},
         {t,f,f,t,f,t},
         {t,t,f,t,t,t},
         {f,t,t,f,t,f},
         {t,t,f,f,t,t}},

        {{f,f,f,t,t,t},
         {t,t,t,t,f,f},
         {f,f,t,f,f,t},
         {f,t,t,t,t,t},
         {t,t,f,f,t,f},
         {f,t,f,t,f,f},
         {f,t,f,t,f,f},
         {f,t,t,t,t,f},
         {f,t,t,f,t,t},
         {t,t,f,f,f,f}},

        {{t,t,t,t,f,t},
         {t,f,f,f,f,t},
         {t,t,f,t,t,t},
         {t,f,f,t,f,f},
         {t,t,f,t,t,f},
         {f,t,t,f,t,t},
         {f,f,t,f,t,f},
         {f,f,t,f,t,t},
         {f,t,t,t,f,t},
         {t,t,f,t,t,t}},

        {{t,t,t,t,f,t},
         {f,f,f,t,f,t},
         {t,t,t,t,f,t},
         {t,f,t,f,f,t},
         {f,t,t,t,f,t},
         {t,t,f,t,t,t},
         {t,f,f,f,t,f},
         {t,t,f,t,t,t},
         {f,t,f,t,f,t},
         {t,t,f,t,f,f}},
        
        {{t,f,t,f,t,t},
         {t,f,t,t,t,f},
         {t,t,t,f,f,f},
         {t,t,f,t,t,f},
         {t,f,t,t,f,t},
         {t,f,t,f,t,t},
         {t,t,t,f,f,t},
         {t,f,t,t,f,t},
         {f,f,f,t,t,t},
         {t,t,t,t,f,t}},
        
         
        //  {{f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f},
        //  {f,f,f,f,f,f}},
    };

    public static Vector2 TilePosition(int x, int y)
    {
        float newx = (-2.5f +x) * tilesize;
        float newy = -1*(1.5f + y) * tilesize;
        return new Vector2(newx, newy);
    }
}
