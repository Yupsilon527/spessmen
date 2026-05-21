using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class PlixelMapMob : Mob
{
    public static int nChunks = 0;
    public int search_index = 0;
    bool completed = false;


    public Vector2 AlphaScale = Vector2.one;
    public PlixelDisplayManager fg, bg;
    public PlixelCollisionManager colFg, colBg;

    public Texture2D referenceTexture;
    public int tilesTotal, tilesSolid = 0;

    public int _width, _height;
    [NonSerialized] public Plixel[] terrain;
    HashSet<ForcePending> consideredforce = new();

    public class ForcePending
    {
        public Vector2 center;
        public Vector2 force;

        public ForcePending(Vector2 center, Vector2 force)
        {
            this.center = center;
            this.force = force;
        }
    }
    #region Initialization
    protected override void Initialize()
    {
        base.Initialize();
        Inspect("[entityTerrain] Enable chunk " + name);
        WorldController.active.terrainmobs.Add(this);
        setComplete(false);
        if (referenceTexture != null)
            Debug.Log("[entityTerrain] Make from tex " + referenceTexture.width + "x" + referenceTexture.height + " chunk " + gameObject.name);

        StartCoroutine(DrawChunks(true));
    }
    public void RepositionAboveWater()
    {
        if (transform.position.y == 0)
        {
            transform.position = Vector3.up * GetHeight() / TerrainDefines.terrain_PPU * 0.5f;
        }
    }
    #endregion
    #region Generation
    public static PlixelMapMob LoadFromTexture(Texture2D terrain)
    {
        Debug.Log("[entityTerrain] Generate Terrain From Texture");
        GameObject newChunk = GameObject.Instantiate(TerrainDefines.TerrainPrefab);
        newChunk.name = "Terra " + nChunks++;
        newChunk.transform.position = Vector3.zero;
        PlixelMapMob terr = newChunk.GetComponent<PlixelMapMob>();
        newChunk.GetComponent<PlixelMapMob>().referenceTexture = terrain;
        newChunk.GetComponent<PlixelMapMob>().terrain = new Plixel[terrain.height * terrain.width];
        newChunk.GetComponent<PlixelMapMob>()._width = terrain.width;
        newChunk.GetComponent<PlixelMapMob>()._height = terrain.height;

        for (int iY = 0; iY < terrain.height; iY++)
        {
            for (int iX = 0; iX < terrain.width; iX++)
            {
                Color colorsolid = terrain.GetPixel(iX, iY);
                int collision = 0;

                if (colorsolid.a > .75f)
                {
                    collision = (int)TerrainDefines.Behavior.Foreground;

                }

                terr.AddTile(new Plixel(terr, iX, iY, terrain.GetPixel(iX, iY), collision));
            }
        }

        bool borders = false;
        if (terr.GetNBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, terrain.width - 1, 1, RetrievePlixelMode.clamped));
            borders = true;
        }
        if (terr.GetSBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, terrain.height - 1, terrain.width - 1, 1, RetrievePlixelMode.clamped));
            borders = true;
        }
        if (terr.GetEBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(terrain.width - 1 - 1, 0, 1, terrain.height - 1, RetrievePlixelMode.clamped));
            borders = true;
        }
        if (terr.GetWBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, 1, terrain.height - 1, RetrievePlixelMode.clamped));
            borders = true;
        }
        if (!borders)
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, terrain.width, terrain.height, RetrievePlixelMode.clamped));
        }
        terr.Initialize();

        return newChunk.GetComponent<PlixelMapMob>();
    }
    public static PlixelMapMob LoadFromTexture(Texture2D terrain, Texture2D mask)
    {
        Debug.Log("[entityTerrain] Generate Terrain From Texture And Mask");
        GameObject newChunk = GameObject.Instantiate(TerrainDefines.TerrainPrefab);
        newChunk.name = "Chunk " + nChunks++;
        newChunk.transform.position = TerrainDefines.terrain_zlayer * Vector3.forward;
        PlixelMapMob terr = newChunk.GetComponent<PlixelMapMob>();
        newChunk.GetComponent<PlixelMapMob>().referenceTexture = terrain;
        newChunk.GetComponent<PlixelMapMob>().terrain = new Plixel[terrain.height * terrain.width];
        newChunk.GetComponent<PlixelMapMob>()._width = terrain.width;
        newChunk.GetComponent<PlixelMapMob>()._height = terrain.height;

        for (int iY = 0; iY < terrain.height; iY++)
        {
            for (int iX = 0; iX < terrain.width; iX++)
            {
                Color colorsolid = mask.GetPixel(iX, iY);
                int collision = 0;

                if (colorsolid.a > .5f)
                {

                    if (colorsolid == Color.white)
                    {
                        collision = (int)TerrainDefines.Behavior.Foreground;
                    }
                    else if (colorsolid == Color.black)
                    {
                        collision = (int)TerrainDefines.Behavior.Background;
                    }
                    else if (colorsolid == Color.red)
                    {
                        collision = (int)TerrainDefines.Behavior.Foreground | (int)TerrainDefines.Behavior.Indestructable;
                    }
                    else if (colorsolid == Color.blue)
                    {
                        collision = (int)TerrainDefines.Behavior.Background | (int)TerrainDefines.Behavior.Indestructable;
                    }
                    else if (colorsolid == Color.green)
                    {
                        collision = (int)TerrainDefines.Behavior.Foreground | (int)TerrainDefines.Behavior.Frozen;
                    }
                    else
                    if (colorsolid == Color.magenta)
                    {
                        collision = (int)TerrainDefines.Behavior.Background | (int)TerrainDefines.Behavior.Frozen;
                    }
                }
                terr.AddTile(new Plixel(terr, iX, iY, terrain.GetPixel(iX, iY), collision));
            }
        }
        terr.Initialize();

        return newChunk.GetComponent<PlixelMapMob>();
    }
    public void setComplete(bool value)
    {
        completed = value;
        rigidbody.mass = (float)tilesTotal * TerrainDefines.terrain_mass_multiplier;
        rigidbody.simulated = value;
    }

    public bool isComplete()
    {
        return !inRevision && completed
            && fg.IsReady()
            && bg.IsReady()
            && colFg.IsReady()
            && colBg.IsReady();
    }
    public List<Vector2> GetValidSpawnLocations(Vector2 playerSize)
    {
        List<Plixel> possible = new List<Plixel>();

        List<Vector2> SpawnLocs = new List<Vector2>();
        Vector2Int SearchSize = Vector2Int.CeilToInt(playerSize * TerrainDefines.terrain_PPU);

        foreach (Plixel Zim in terrain)
        {
            if (Zim != null && Zim.CanSpawnEntity())
            { possible.Add(Zim); }
        }

        foreach (Plixel Zim in possible)
        {
            bool valid = true;

            for (Vector2 vRect = Vector2.zero; valid && (vRect.y <= SearchSize.y);)
            {
                Vector2Int center = Vector2Int.RoundToInt(Zim.position + Vector2.up + vRect - Vector2.right * SearchSize.x / 2f);

                Plixel Gir = GetTileAt(center.x, center.y, RetrievePlixelMode.clamped);
                if (Gir != null && Gir.IsForeGround())
                { valid = false; }
                if (vRect.x <= SearchSize.x)
                {
                    vRect.x++;
                }
                else { vRect.x = 0; vRect.y++; }
            }
            if (valid)
            {
                SpawnLocs.Add(tiletoworldPosition(Zim.position) + (Vector2.up + Vector2.right / 2) / TerrainDefines.terrain_PPU);
            }
        }
        return SpawnLocs;
    }
    #endregion
    #region Destroy Tiles
    public PlixelMapMob FromPlixels(Plixel[] chunk)
    {
        Debug.Log("[entityTerrain] Generate Terrain From Chunk Data");
        Rect bounds = new Rect(chunk[0].position.x, chunk[0].position.y, 0, 0);

        foreach (Plixel Zim in chunk)
        {
            bounds.xMin = Mathf.Min(Zim.position.x, bounds.xMin);
            bounds.xMax = Mathf.Max(Zim.position.x, bounds.xMax);
            bounds.yMin = Mathf.Min(Zim.position.y, bounds.yMin);
            bounds.yMax = Mathf.Max(Zim.position.y, bounds.yMax);
        }

        Plixel[,] terrainArray = new Plixel[(int)bounds.width + 1, (int)bounds.height + 1];

        foreach (Plixel zim in chunk)
        {
            int x = Mathf.FloorToInt(zim.position.x - bounds.xMin);
            int y = Mathf.FloorToInt(zim.position.y - bounds.yMin);

            terrainArray[x, y] = zim;
        }

        GameObject newChunk = Instantiate(TerrainDefines.TerrainPrefab);
        newChunk.name = "Terra " + nChunks++;

        PlixelMapMob eChink = newChunk.GetComponent<PlixelMapMob>();
        eChink._width = (int)bounds.width + 1;
        eChink._height = (int)bounds.height + 1;
        eChink.terrain = new Plixel[eChink._height * eChink._width];

        for (int iY = 0; iY < terrainArray.GetLength(1); iY++)
        {
            for (int iX = 0; iX < terrainArray.GetLength(0); iX++)
            {
                if (terrainArray[iX, iY] != null)
                    eChink.AddTile(terrainArray[iX, iY].Duplicate(eChink, iX, iY));
            }
        }

        Vector2 center = new Vector2(bounds.center.x + .5f, bounds.center.y + .5f) - GetWorldSize() / 2f;
        newChunk.transform.position = transform.position + (transform.right * center.x + transform.up * center.y) / TerrainDefines.terrain_PPU;
        newChunk.transform.rotation = transform.rotation;

        TransferForce(eChink);
        eChink.Initialize();

        return eChink;
    }
    void TransferForce(Mob other)
    {

        var newRigidBody = other.GetComponent<Rigidbody2D>();
        newRigidBody.isKinematic = true;
        newRigidBody.angularVelocity = GetComponent<Rigidbody2D>().angularVelocity;
        newRigidBody.velocity = GetComponent<Rigidbody2D>().velocity;

        if (consideredforce.Count > 0)
            foreach (var f in consideredforce)
                other.ApplyForce(f.force, f.center);
    }
    #endregion
    #region Alter Tile
    public void AddTile(Plixel tile)
    {
        AddTile(tile.position.x, tile.position.y, tile);
    }

    public void AddTile(int iX, int iY, Plixel tile)
    {
        terrain[iY * _width + iX] = tile;
        if (tile != null && tile.IsSolid())
        {
            tilesTotal++;
            if (tile.getFrozen())
            {
                tilesSolid++;
            }
        }
    }
    public void FreezeTiles(Plixel[] Tiles)
    {
        foreach (Plixel t in Tiles)
        {
            if (!t.getFrozen())
            {
                t.SetFrozen();
                tilesSolid++;
            }
        }
    }
    public Vector2 GetWorldSize()
    { return new Vector2(_width, _height); }

    public bool GetNBounds()
    {
        foreach (Plixel tile in GetTilesInRect(0, 0, _width, 1, RetrievePlixelMode.clamped))
        {
            if (tile.IsSolid())
            {
                return true;
            }
        }
        return false;
    }
    public bool GetSBounds()
    {
        foreach (Plixel tile in GetTilesInRect(0, _height - 1, _width, 1, RetrievePlixelMode.clamped))
        {
            if (tile.IsSolid())
            {
                return true;
            }
        }
        return false;
    }
    public bool GetEBounds()
    {
        foreach (Plixel tile in GetTilesInRect(_width - 1, 0, 1, _height, RetrievePlixelMode.clamped))
        {
            if (tile.IsSolid())
            {
                return true;
            }
        }
        return false;
    }
    public bool GetWBounds()
    {
        foreach (Plixel tile in GetTilesInRect(0, 0, 1, _height, RetrievePlixelMode.clamped))
        {
            if (tile.IsSolid())
            {
                return true;
            }
        }
        return false;
    }
    public int GetWidth()
    {
        return _width;
    }
    public int GetHeight()
    {
        return _height;
    }
    public bool isSolid()
    {
        return tilesSolid > 0;
    }


    #endregion



    #region Collision
    public List<List<Vector2>> ReviseCollisionShape(Vector2 center, bool[,] segment)
    {
        Vector2 origin = new Vector2(_width, _height) / 2;

        List<PixelColliderSegment> collider_points = new List<PixelColliderSegment>();
        List<List<PixelColliderSegment>> collider_points_final = new List<List<PixelColliderSegment>>();

        int w = segment.GetLength(1);
        int height = segment.GetLength(0);

        List<Vector2> solidCoords = new List<Vector2>();
        for (int x = 1; x < height - 1; x++)
        {
            for (int y = 1; y < w - 1; y++)
            {
                bool solid = segment[x, y];
                if (solid)
                {
                    //bool hBorder = x == width - 1 || x == 0;
                    //bool vBorder = y == height - 1 || y == 0;
                    var points = new List<PixelColliderSegment>();
                    Vector2 pos = new Vector2(y, x) + center - origin + new Vector2(.5f, .5f);
                    if (y == height - 2 || !segment[x, y + 1])//r
                    {
                        points.Add(new PixelColliderSegment(pos + (Vector2.right + Vector2.up) / 2, pos + (Vector2.right + Vector2.down) / 2));
                    }
                    if (y == 1 || !segment[x, y - 1])//l
                    {
                        points.Add(new PixelColliderSegment(pos + (Vector2.left + Vector2.up) / 2, pos + (Vector2.left + Vector2.down) / 2));
                    }
                    if (!segment[x + 1, y])//u
                    {
                        points.Add(new PixelColliderSegment(pos + (Vector2.up + Vector2.left) / 2, pos + (Vector2.up + Vector2.right) / 2));
                    }
                    if (!segment[x - 1, y])
                    {
                        points.Add(new PixelColliderSegment(pos + (Vector2.down + Vector2.left) / 2, pos + (Vector2.down + Vector2.right) / 2));
                    }


                    //    if (hBorder || !segment[x, y + 1] || !segment[x, y - 1] || vBorder || !segment[x + 1, y] || !segment[x - 1, y])
                    // {
                    //     solidCoords.Add(new Vector2(y, x) + center - origin);x
                    //  }
                    collider_points.AddRange(points);
                }
            }
        }


        /*// Iterate through all the coordinates in solidCoords
        for (int i = 0; i < solidCoords.Count; i++)
        {
            Vector2 start = solidCoords[i];

            // Compare with the rest of the coordinates
            for (int j = i + 1; j < solidCoords.Count; j++)
            {
                Vector2 end = solidCoords[j];
                Vector2 delta = start - end;
                // Check if the distance between two points is less than or equal to 1 unit
                if (delta .sqrMagnitude > 0 && Mathf.Abs(delta.x)<=1 && Mathf.Abs(delta.y) <= 1)
                {
                    // Create a new PixelColliderSegment between start and end points
                    PixelColliderSegment pcs = new PixelColliderSegment(start, end);

                    // Add it to the vectorlist
                    collider_points.Add(pcs);
                }
            }
        }*/

        List<PixelColliderSegment> vectorlist = new List<PixelColliderSegment>();
        while (collider_points.Count > 0)
        {
            // Declare start and add to the vectorlist
            PixelColliderSegment current = collider_points[0];
            vectorlist.Add(current);

            // Remove the first element (current segment)
            collider_points.RemoveAt(0);

            bool foundNeighbor;

            // Loop until no more neighbors are found
            do
            {
                foundNeighbor = false;

                for (int i = 0; i < collider_points.Count; i++)
                {
                    PixelColliderSegment next = collider_points[i];
                    int neigh_id = current.isNeighboring(next);

                    if (neigh_id != 0)
                    {

                        // If segments can't be merged, add the next one to the correct position
                        if (!current.Merge(next))
                        {
                            if (neigh_id > 0)
                            {
                                vectorlist.Add(next);  // Add to the end of the list
                            }
                            else
                            {
                                vectorlist.Insert(0, next);  // Insert at the beginning
                            }
                            // Update the current segment
                            current = next;
                        }


                        // Remove the processed segment
                        collider_points.RemoveAt(i);
                        foundNeighbor = true;
                        break; // Exit the loop and restart the neighbor search
                    }
                }

            } while (foundNeighbor);
            CloseShape(ref vectorlist);

            // Once finished with this chain, add it to the final list
            collider_points_final.Add(vectorlist);
            vectorlist = new List<PixelColliderSegment>();  // Reset for the next segment group
        }


        List<List<Vector2>> final_polygons = new List<List<Vector2>>();
        foreach (List<PixelColliderSegment> Gir in collider_points_final)
        {
            List<Vector2> final_polygon = new List<Vector2>();
            foreach (PixelColliderSegment Zim in Gir)
            {
                if (!final_polygon.Contains(Zim.start))
                {
                    final_polygon.Add(Zim.start);
                }
                if (!final_polygon.Contains(Zim.end))
                {
                    final_polygon.Add(Zim.end);
                }
            }
            for (int Zim = 0; Zim < final_polygon.Count; Zim++)
            {
                final_polygon[Zim] /= TerrainDefines.terrain_PPU;
            }

            final_polygons.Add(final_polygon);
        }

        return final_polygons;
    }
    public void CloseShape(ref List<PixelColliderSegment> segments)
    {
        if (segments == null || segments.Count == 0) return; // Handle empty list

        List<PixelColliderSegment> sortedList = new List<PixelColliderSegment> { segments[0] };
        segments.RemoveAt(0);

        while (segments.Count > 0)
        {
            PixelColliderSegment lastSegment = sortedList[sortedList.Count - 1];
            Vector2 lastEnd = lastSegment.end;

            // Variable to track the closest segment
            PixelColliderSegment closestSegment = null;
            float closestDistance = float.MaxValue;
            bool flipDirection = false;
            int closestIndex = -1;

            // Find the closest segment
            for (int i = 0; i < segments.Count; i++)
            {
                PixelColliderSegment currentSegment = segments[i];

                // Calculate distances for both directions (start vs end)
                float distanceToStart = Vector2.Distance(lastEnd, currentSegment.start);
                float distanceToEnd = Vector2.Distance(lastEnd, currentSegment.end);

                if (distanceToStart < closestDistance)
                {
                    closestDistance = distanceToStart;
                    closestSegment = currentSegment;
                    flipDirection = false;
                    closestIndex = i;
                }

                if (distanceToEnd < closestDistance)
                {
                    closestDistance = distanceToEnd;
                    closestSegment = currentSegment;
                    flipDirection = true;
                    closestIndex = i;
                }
            }

            // Flip the segment if needed
            if (flipDirection)
            {
                closestSegment.FlipDirection();
            }

            // Add the closest segment to the sorted list
            sortedList.Add(closestSegment);
            segments.RemoveAt(closestIndex);  // Remove the segment from the unsorted list
        }
        segments = sortedList;
    }
    public Vector2 tiletoworldPosition(Vector2Int pos)
    {
        float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 center = new Vector2(_width, _height) / 2f;
        Vector2 npos = new Vector2(
            pos.x - center.x,
            pos.y - center.y
            ) / TerrainDefines.terrain_PPU;

        npos = new Vector2(
            -npos.y * Mathf.Sin(rotation) + npos.x * Mathf.Cos(rotation),
            npos.x * Mathf.Sin(rotation) + npos.y * Mathf.Cos(rotation)
            );

        return npos + (Vector2)transform.position;
    }
    #endregion
    #region Translation
    public Vector2Int worldtotilePosition(Vector2 pos)
    {
        float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

        pos = pos - (Vector2)transform.position;
        pos = new Vector2(
            pos.y * Mathf.Sin(rotation) + pos.x * Mathf.Cos(rotation),
            -pos.x * Mathf.Sin(rotation) + pos.y * Mathf.Cos(rotation)
            );

        Vector2 center = new Vector2(_width, _height) / 2f;
        return Vector2Int.RoundToInt(new Vector2(
            pos.x * TerrainDefines.terrain_PPU + center.x,
            pos.y * TerrainDefines.terrain_PPU + center.y
            ));
    }
    public Vector2Int tiletochunkPosition(int x, int y)
    {
        return new Vector2Int(x, y) / TerrainDefines.terrain_chunk_size;
    }

    public Vector2Int mousetotilePosition()
    {

        return worldtotilePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
    }
    #endregion
    #region Gets
    public enum RetrievePlixelMode
    {
        normal,
        imaginary,
        clamped,
        quick
    }
    public Plixel GetTileAt(int ix, int iy, RetrievePlixelMode mode)
    {
        switch (mode)
        {
            case RetrievePlixelMode.quick:
                return GetTileAt(ix, iy);
            case RetrievePlixelMode.normal:
                if (ix >= 0 && iy >= 0 && ix < _width && iy < _height)
                    return GetTileAt(ix, iy);
                break;
            case RetrievePlixelMode.imaginary:
                if (ix >= 0 && iy >= 0 && ix < _width && iy < _height && GetTileAt(ix, iy) is Plixel pl)
                    return pl;
                return new Plixel(this);
            case RetrievePlixelMode.clamped:
                ix = Mathf.Clamp(ix, 0, GetWidth() - 1);
                iy = Mathf.Clamp(iy, 0, GetHeight() - 1);
                return GetTileAt(ix, iy);
        }
        return null;
    }
    public Plixel GetTileAt(int ix, int iy)
    {
        return terrain[iy * _width + ix];
    }

    public Color GetColorAt(int ix, int iy)
    {
        return referenceTexture.GetPixel(ix, iy);
    }



    public Plixel[] GetTilesInRect(RectInt r, RetrievePlixelMode mode)
    {
        return GetTilesInRect(r.xMin, r.yMin, r.width, r.height, mode);
    }
    public Plixel[] GetTilesInRect(int x, int y, int w, int h, RetrievePlixelMode mode)
    {
        List<Plixel> value = new List<Plixel>();

        for (int iY = 0; iY <= 0 + h; iY++)
        {
            for (int iX = 0; iX <= 0 + w; iX++)
            {
                Plixel tileAt = GetTileAt(iX + x, iY + y, mode);
                if (tileAt != null)
                {
                    value.Add(tileAt);
                }

            }
        }
        return value.ToArray();
    }

    public Plixel[] GetTilesInCircle(int x, int y, int r, RetrievePlixelMode mode = RetrievePlixelMode.normal, bool solid = false)
    {
        return GetTilesInCircle(new Vector2Int(x, y), r, mode, solid);
    }

    public Plixel[] GetTilesInCircle(Vector2Int c, int r, RetrievePlixelMode mode = RetrievePlixelMode.normal, bool solid = false)
    {
        List<Plixel> value = new List<Plixel>();

        int radiusSquared = r * r;
        for (int iX = 0 - r; iX <= 0 + r; iX++)
        {
            int xSquared = iX * iX;
            for (int iY = 0 - r; iY <= 0 + r; iY++)
            {
                int ySquared = iY * iY;
                if (xSquared + ySquared <= radiusSquared)
                {
                    Plixel Zim = GetTileAt(iX + c.x, iY + c.y, mode);
                    if (Zim != null && (!solid || Zim.IsSolid()))
                    {
                        value.Add(Zim);
                    }
                }
            }
        }
        return value.ToArray();
    }
    #endregion
    #region Drawing
    public IEnumerator DrawChunks(bool instant)
    {
        setComplete(false);

        Debug.Log("[entityTerrain] Begin couroutine DrawChunks for " + name);
        fg?.OnCreated();
        bg?.OnCreated();
        yield return null;



        Debug.Log("[entityTerrain] Initialize colliders for " + name);

        if (colFg != null)
            colFg.OnCreated();
        yield return null;
        if (colBg != null)
            colBg.OnCreated();

        yield return null;
        ModifyTiles(GetRectBounds());

    }
    #endregion

    #region Destruction
    public void HandleExplosion(ExplosionData explosion)
    {
        HandleExplosion(explosion.center, explosion.inner_radius, explosion.inner_damage, explosion.middle_radius, explosion.middle_damage, explosion.outer_radius, explosion.outer_damage);

    }

    public void HandleExplosion(Vector2 world_position, float inner_radius, int inner_damage, float middle_radius, int middle_damage, float outer_radius, int outer_damage)
    {
        Vector2Int explosion_center = worldtotilePosition(world_position);
        int iOuterRadius = Mathf.CeilToInt(outer_radius * TerrainDefines.terrain_PPU);

        HashSet<Plixel> damagedTiles = new();

        foreach (Plixel tile in GetTilesInCircle(explosion_center.x, explosion_center.y, iOuterRadius, RetrievePlixelMode.normal))
        {
            if (tile == null) continue;
            float distance = (tile.position - explosion_center).sqrMagnitude;

            int damage = 0;

            if (inner_radius > 0 && distance < inner_radius * TerrainDefines.terrain_PPU)
                damage = inner_damage;
            else if (middle_radius > 0 && distance < middle_radius * TerrainDefines.terrain_PPU)
                damage = middle_damage;
            else if (outer_radius > 0)
                damage = outer_damage;

            if (TakeDamage(tile, damage, Revision.everything))
            {
                damagedTiles.Add(tile);
            }
        }
        ModifyTiles(explosion_center.x, explosion_center.y, iOuterRadius);
    }

    public void HandleExplosionCoroutine(Vector2 world_position, float inner_radius, int inner_damage, float middle_radius, int middle_damage, float outer_radius, int outer_damage)
    {
        Vector2Int explosion_center = worldtotilePosition(world_position);
        int iOuterRadius = Mathf.CeilToInt(outer_radius * TerrainDefines.terrain_PPU);

        if ((explosion_center.x + iOuterRadius >= 0 || explosion_center.x - iOuterRadius < _height) && (explosion_center.y + iOuterRadius >= 0 || explosion_center.y - iOuterRadius < _width))
        {

            int radiusSquared = iOuterRadius * iOuterRadius;


            HashSet<Plixel> explodedTiles = new();
            for (int iX = 0 - iOuterRadius; iX <= 0 + iOuterRadius + 1; iX++)
            {
                int xSquared = iX * iX;
                for (int iY = 0 - iOuterRadius; iY <= 0 + iOuterRadius + 1; iY++)
                {
                    int ySquared = iY * iY;
                    float distance = (xSquared + ySquared);
                    if (distance <= radiusSquared)
                    {
                        Plixel tile = GetTileAt(iX + explosion_center.x, iY + explosion_center.y, RetrievePlixelMode.normal);
                        if (tile != null && tile.IsSolid() && !tile.IsIndestructable())
                        {
                            float dirty;
                            if (distance > middle_radius * middle_radius)
                            {
                                dirty = (1 - (Mathf.Sqrt(distance) - middle_radius) / (outer_radius - middle_radius)) * 99;
                            }
                            else
                            {
                                dirty = (distance < inner_radius * inner_radius) ? 200 : 100;
                            }

                            tile.Damage(dirty);
                            explodedTiles.Add(tile);
                        }
                    }
                }
            }
            ModifyTiles(explosion_center.x, explosion_center.y, iOuterRadius);
        }
    }
    public void StainTiles(Vector2 world_position, float radius, Color32 color, bool stainFG, bool stainBG, bool apply = true)
    {
        Vector2Int explosion_center = worldtotilePosition(world_position);
        int iradius = Mathf.CeilToInt(radius * TerrainDefines.terrain_PPU);

        var affected = GetTilesInCircle(explosion_center.x, explosion_center.y, iradius, RetrievePlixelMode.normal, true);
        foreach (Plixel Zim in affected)
        {
            if (Zim == null) continue;
            if (stainFG || stainBG)
            {
                Zim.ChangeColor(color);

                if (stainFG && !stainBG)
                {
                    Zim.stain = Zim.stain == Plixel.StainState.bg ? Plixel.StainState.both : Plixel.StainState.fg;
                }
                else if (!stainFG && stainBG)
                {
                    Zim.stain = Zim.stain == Plixel.StainState.fg ? Plixel.StainState.both : Plixel.StainState.bg;
                }
                else if (stainFG && stainBG)
                {
                    Zim.stain = Plixel.StainState.both;
                }
                // Zim.UpdateRealColor();
            }
        }
        if (apply)
            ModifyTiles(explosion_center.x, explosion_center.y, iradius, Revision.visual);
    }
    public bool TakeDamage(Plixel tile, float dirty, Revision revise)
    {

        if (tile.IsSolid() && !tile.IsIndestructable())
        {
            if ((tile.position.x >= 0 && tile.position.y >= 0 && tile.position.x < _width && tile.position.y < _height))
            {
                tile.Damage(dirty);
                //DELETE TILE HERE
                return true;
            }
        }
        return false;
    }
    public void DestroyTiles(Plixel[] tiles, Revision revises, bool final)
    {
        if (tiles.Length > 0)
        {
            foreach (Plixel tile in tiles)
            {
                tile.Kill(true);
            }
        }
        ModifyTiles(tiles, revises, final);
    }
    #endregion
    #region Force
    public override void ApplyForce(Vector2 force, Vector2 center)
    {

        if (isComplete())
        {
            rigidbody.AddForceAtPosition(force, center);
            WorldController.active.MobsInMotion.Add(this);
        }
        else
        {
            consideredforce.Add(new ForcePending(center, force));
        }
        Debug.Log("[PlixelMapMob] Apply " + force + " force to " + name + " at point " + center);

    }
    #endregion
    #region Revision
    bool inRevision = false;
    RectInt dirtyRect = default;

    public enum Revision
    {
        none,
        visual,
        everything
    }
    RectInt GetRectBounds()
    {
        return new RectInt(0, 0, GetWidth(), GetHeight());
    }
    public void ModifyTiles(Plixel[] affected, Revision revisionType = Revision.everything, bool final = true)
    {
        if (!inRevision)
        {
            dirtyRect = GetRectBounds();

            dirtyRect.xMin = dirtyRect.xMax;
            dirtyRect.yMin = dirtyRect.yMax;
            dirtyRect.xMax = 0;
            dirtyRect.yMax = 0;
            inRevision = true;
        }


        foreach (Plixel plix in affected)
        {
            dirtyRect.xMin = Math.Min(plix.position.x, dirtyRect.xMin);
            dirtyRect.yMin = Math.Min(plix.position.y, dirtyRect.yMin);
            dirtyRect.xMax = Math.Max(plix.position.x, dirtyRect.xMax);
            dirtyRect.yMax = Math.Max(plix.position.y, dirtyRect.yMax);
        }
        if (final) ModifyTiles(revisionType);
    }
    public void ModifyTiles(int centerX, int centerY, int radius, Revision revisionType = Revision.everything)
    {
        ModifyTiles(new RectInt(centerX - radius, centerY - radius, centerX + radius, centerY + radius), revisionType);
    }
    public void ModifyTiles(RectInt rect, Revision revisionType = Revision.everything, bool final = true)
    {
        if (!inRevision)
        {
            dirtyRect = rect;
            inRevision = true;
        }
        else
        {
            dirtyRect.xMin = Math.Min(rect.xMin, dirtyRect.xMin);
            dirtyRect.yMin = Math.Min(rect.yMin, dirtyRect.yMin);
            dirtyRect.xMax = Math.Max(rect.xMax, dirtyRect.xMax);
            dirtyRect.yMax = Math.Max(rect.yMax, dirtyRect.yMax);
        }
        if (final) ModifyTiles(revisionType);
    }
    public void ModifyTiles(Revision revisionType = Revision.everything)
    {
        if (revisionType > Revision.none)
        {
            colFg?.UpdateSolidState();
            colBg?.UpdateSolidState();

            Debug.Log("Visual Revision " + name);

            if (revisionType > Revision.none)
            {
                fg?.NotifyModified(dirtyRect);
                bg?.NotifyModified(dirtyRect);
            }
            Debug.Log("Collision Revision " + name);
            if (revisionType > Revision.visual)
            {
                colFg?.NotifyModified(dirtyRect);
                colBg?.NotifyModified(dirtyRect);
            }
            EndRevision();
        }
    }
    void EndRevision()
    {
        inRevision = false;

        foreach (var force in consideredforce.ToArray())
        {
            ApplyForce(force.force, force.center);
        }
        consideredforce.Clear();

        if (!completed)
            setComplete(true);
    }
    #endregion
    protected override void HandleOrbit(bool forced)
    {
        if (Planet.gameObject != gameObject)
            base.HandleOrbit(forced);
    }
}