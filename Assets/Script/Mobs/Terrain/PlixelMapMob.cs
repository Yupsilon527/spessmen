using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlixelMapMob : Mob
{
    public int search_index = 0;
    bool solid = false;
    bool completed = false;
    Coroutine RevisionCoroutine;
    Texture2D referenceTexture;

    public PlixelChunk[,] chunk_data;

    Plixel[,] terrain;
    public List<PlixelChunk> chunk_updated = new List<PlixelChunk>();
    List<Plixel> tile_revision = new List<Plixel>();
    Vector4 consideredforce = Vector4.zero;

    public static PlixelMapMob LoadFromTexture(Texture2D terrain)
    {
        Debug.Log("[entityTerrain] Generate Terrain From Texture");
        GameObject newChunk = GameObject.Instantiate(TerrainDefines.TerrainPrefab);
        newChunk.transform.position = TerrainDefines.terrain_zlayer * Vector3.forward;
        PlixelMapMob terr = newChunk.GetComponent<PlixelMapMob>();
        newChunk.GetComponent<PlixelMapMob>().terrain = new Plixel[terrain.height, terrain.width];

        for (int iY = 0; iY < terrain.height; iY++)
        {
            for (int iX = 0; iX < terrain.width; iX++)
            {
                Color colorsolid = terrain.GetPixel(iX, iY);

                terr.AddTile(new Plixel(terr, iX, iY, terrain.GetPixel(iX, iY), colorsolid.a > .75f));
            }
        }

        bool borders = false;
        if (terr.GetNBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, terrain.width - 1, 1, false));
            borders = true;
        }
        if (terr.GetSBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, terrain.height - 1, terrain.width - 1, 1, false));
            borders = true;
        }
        if (terr.GetEBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(terrain.width - 1 - 1, 0, 1, terrain.height - 1, false));
            borders = true;
        }
        if (terr.GetWBounds())
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, 1, terrain.height - 1, false));
            borders = true;
        }
        if (!borders)
        {
            terr.FreezeTiles(terr.GetTilesInRect(0, 0, terrain.width, terrain.height, false));
        }
        terr.enabled = true;

        return newChunk.GetComponent<PlixelMapMob>();
    }
    protected override void HandleOrbit(bool forced)
    {
        if (Planet.gameObject != gameObject)
            base.HandleOrbit(forced);
    }

    public PlixelMapMob BreakChunk(Plixel[] chunk)
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

        GameObject newChunk = GameObject.Instantiate(TerrainDefines.TerrainPrefab);
        PlixelMapMob eChink = newChunk.GetComponent<PlixelMapMob>();
        eChink.terrain = new Plixel[(int)bounds.height + 1, (int)bounds.width + 1];
        eChink.HandleShockwave(new Vector2(consideredforce.x, consideredforce.y), 0, consideredforce.z, consideredforce.w,0);//TODO

        for (int iY = 0; iY < bounds.height + 1; iY++)
        {
            for (int iX = 0; iX < bounds.width + 1; iX++)
            {
                eChink.AddTile(new Plixel(eChink, iX, iY, Color.clear, false));
            }
        }
        foreach (Plixel Gir in chunk)
        {
            Vector2Int newpos = Gir.position - Vector2Int.RoundToInt(new Vector2(bounds.xMin, bounds.yMin));
            eChink.terrain[newpos.y, newpos.x].CloneTile(Gir);
        }

        Vector2 center = new Vector2(bounds.center.x + .5f, bounds.center.y + .5f) - GetWorldSize() / 2f;
        newChunk.transform.position = transform.position + (transform.right * center.x + transform.up * center.y) / TerrainDefines.terrain_PPU;
        newChunk.transform.rotation = transform.rotation;

        newChunk.GetComponent<Rigidbody2D>().angularVelocity = GetComponent<Rigidbody2D>().angularVelocity;
        newChunk.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;

        //eChink.setComplete(false);
        eChink.enabled = true;

        return eChink;
    }

    public void AddTile(Plixel tile)
    {
        AddTile(tile.position.x, tile.position.y, tile);
    }

    public void AddTile(int iX, int iY, Plixel tile)
    {
        terrain[iY, iX] = tile;
    }

    public void setComplete(bool value)
    {
        completed = value;
        rigidbody.mass = (float)nTiles * TerrainDefines.terrain_mass_multiplier;
        rigidbody.simulated = value;
    }

    public bool isComplete()
    {
        return RevisionCoroutine == null && completed;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log("[entityTerrain] Enable chunk " + name);
        WorldController.active.terrainmobs.Add(this);
        setComplete(false);
    }

    private void OnEnable()
    {

        DrawTexture();
        Debug.Log("[entityTerrain] Make " + referenceTexture.width + "x" + referenceTexture.height + " chunk " + gameObject.name);
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(referenceTexture, new Rect(0, 0, referenceTexture.width, referenceTexture.height), Vector2.one / 2f, TerrainDefines.terrain_PPU);
        StartCoroutine(DrawChunks());
    }


    public IEnumerator DrawChunks()
    {
        setComplete(false);

        Debug.Log("[entityTerrain] Begin couroutine DrawChunks for " + name);
        for (int iY = 0; iY < terrain.GetLength(0); iY++)
        {
            for (int iX = 0; iX < terrain.GetLength(1); iX++)
            {
                if (terrain[iY, iX] != null && terrain[iY, iX].IsSolid())
                {
                    tile_revision.Add(terrain[iY, iX]);
                }
            }
        }
        StartUpdateTiles(true);
        while (RevisionCoroutine != null)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        Debug.Log("[entityTerrain] Initialize colliders for " + name);
        float StartTime = Time.realtimeSinceStartup;

        GameObject[] colliders = new GameObject[] {
            transform.GetChild(0).gameObject,
            transform.GetChild(1).gameObject
    };

        int wX = Mathf.CeilToInt((float)terrain.GetLength(1) / (float)TerrainDefines.terrain_chunk_size);
        int wY = Mathf.CeilToInt((float)terrain.GetLength(0) / (float)TerrainDefines.terrain_chunk_size);
        int count = 0;
        chunk_data = new PlixelChunk[wY, wX];

        for (int iY = 0; iY < wY; iY++)
        {
            for (int iX = 0; iX < wX; iX++)
            {
                if (count > TerrainDefines.terrain_ram_size)
                {
                    count = 0;
                    Debug.Log(gameObject.name + " drawing...");
                    yield return new WaitForEndOfFrame();
                }
                count += TerrainDefines.terrain_chunk_size * TerrainDefines.terrain_chunk_size;
                PlixelChunk box = new PlixelChunk(this, new Vector2Int(iX, iY), TerrainDefines.terrain_chunk_size, colliders);
                box.Revise();
                chunk_data[iY, iX] = (box);
            }

        }

        yield return new WaitForEndOfFrame();
        Debug.Log("[entityTerrain] " + (Time.realtimeSinceStartup - StartTime) + " total needed to draw " + gameObject.name);

        setComplete(true);
    }

    protected int nTiles = 0;

    public Plixel GetTileAt(int ix, int iy, bool imaginary)
    {
        if (ix < 0 || iy < 0 || ix >= terrain.GetLength(1) || iy >= terrain.GetLength(0) || terrain[iy, ix] == null)
        {
            if (!imaginary)
            { return null; }
            return new Plixel(this);
        }
        return terrain[iy, ix];
    }

    public Color GetColorAt(int ix, int iy)
    {
        return referenceTexture.GetPixel(ix, iy);
    }

    public List<List<Vector2>> ReviseCollision(Rect segment, bool foregroundlayer)
    {
        EdgeCollider2D collider = GetComponent<EdgeCollider2D>();
        Vector2 center = new Vector2(terrain.GetLength(1), terrain.GetLength(0)) / 2;

        List<PixelColliderSegment> collider_points = new List<PixelColliderSegment>();
        List<List<PixelColliderSegment>> collider_points_final = new List<List<PixelColliderSegment>>();

        for (int height = (int)segment.yMin; height <= segment.yMax; height++)
        {
            for (int width = (int)segment.xMin; width <= segment.xMax; width++)
            {
                Plixel tile = GetTileAt(width, height, true);
                if (tile.IsRelevant(foregroundlayer) || (tile.IsSolid(foregroundlayer) && (

                    tile.position.x <= segment.xMin ||
                    tile.position.x >= segment.xMax ||
                    tile.position.y <= segment.yMin ||
                    tile.position.y >= segment.yMax

                    )))
                {
                    collider_points.AddRange(tile.getCollision(segment, center, foregroundlayer));
                }

            }
        }

        List<PixelColliderSegment> vectorlist = new List<PixelColliderSegment>();
        while (collider_points.Count > 0)
        {

            //start
            vectorlist.Add(collider_points[0]);
            collider_points.RemoveAt(0);

            //declare start and next
            PixelColliderSegment current = vectorlist[vectorlist.Count - 1];

        loop_start:
            bool concluded = true;

            for (int integ = 0; integ < collider_points.Count; integ++)
            {
                PixelColliderSegment next = collider_points[integ];
                int neigh_id = current.isNeighboring(next);
                if (neigh_id != 0)
                {
                    if (!current.Merge(next))
                    {
                        if (Mathf.Abs(neigh_id) > 1)
                        {
                            next.FlipDirection();
                        }
                        if (neigh_id > 0)
                        {
                            vectorlist.Add(next);
                            current = next;
                        }
                        else
                        {
                            vectorlist.Insert(vectorlist.IndexOf(current), next);
                            current = next;
                        }
                        collider_points.Remove(current);
                    }
                    else
                    {
                        collider_points.Remove(next);
                    }

                    concluded = false;
                    break;
                }
            }
            if (!concluded)
            { goto loop_start; }
            else
            {
                collider_points_final.Add(vectorlist);
                vectorlist = new List<PixelColliderSegment>();
            }
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
    public Vector2 tiletoworldPosition(Vector2Int pos)
    {
        float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

        Vector2 center = new Vector2(terrain.GetLength(1), terrain.GetLength(0)) / 2f;
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


    public Vector2Int worldtotilePosition(Vector2 pos)
    {
        float rotation = transform.eulerAngles.z * Mathf.Deg2Rad;

        pos = pos - (Vector2)transform.position;
        pos = new Vector2(
            pos.y * Mathf.Sin(rotation) + pos.x * Mathf.Cos(rotation),
            -pos.x * Mathf.Sin(rotation) + pos.y * Mathf.Cos(rotation)
            );

        Vector2 center = new Vector2(terrain.GetLength(1), terrain.GetLength(0)) / 2f;
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

    public PlixelChunk getChunkAt(Vector2Int pos)
    {
        return getChunkAt(pos.x, pos.y);
    }

    public PlixelChunk getChunkAt(int x, int y)
    {
        if (completed && chunk_data == null)
        {
            Debug.LogError("Chunk " + gameObject.name + " has no chunk data, but tile count " + nTiles);
            return null;
        }
        else if (chunk_data != null)
        {
            return chunk_data[
                Mathf.Clamp(y, 0, chunk_data.GetLength(0) - 1)
                ,
                Mathf.Clamp(x, 0, chunk_data.GetLength(1) - 1)
                ];
        }

        return null;
    }

    public Plixel[] GetTilesInRect(int x, int y, int w, int h, bool imaginary)
    {
        List<Plixel> value = new List<Plixel>();

        for (int iX = 0; iX <= 0 + w; iX++)
        {
            for (int iY = 0; iY <= 0 + h; iY++)
            {
                Plixel tileAt = GetTileAt(iX + x, iY + y, imaginary);
                if (tileAt != null)
                {
                    value.Add(tileAt);
                }

            }
        }
        return value.ToArray();
    }

    public Plixel[] GetTilesInCircle(int x, int y, int r, bool imaginary)
    {
        return GetTilesInCircle(new Vector2Int(x, y), r, imaginary);
    }

    public Plixel[] GetTilesInCircle(Vector2Int c, int r, bool imaginary)
    {
        List<Plixel> value = new List<Plixel>();

        for (int iX = 0 - r; iX <= 0 + r; iX++)
        {
            for (int iY = 0 - r; iY <= 0 + r; iY++)
            {
                if (new Vector2(iX, iY).sqrMagnitude <= r * r)
                {
                    Plixel Zim = GetTileAt(iX + c.x, iY + c.y, imaginary);
                    if (Zim != null)
                    {
                        value.Add(Zim);
                    }
                }
            }
        }
        return value.ToArray();
    }

    public void HandleExplosion(ExplosionData explosion)
    {
        HandleExplosion(explosion.center, explosion.inner_radius, explosion.inner_damage,explosion.middle_radius, explosion.middle_damage, explosion.outer_radius, explosion.outer_damage);

    }

        public void HandleExplosion(Vector2 world_position, float inner_radius, int inner_damage, float middle_radius, int middle_damage, float outer_radius, int outer_damage)
    {
        Vector2Int explosion_center = worldtotilePosition(world_position);

        foreach (Plixel Zim in GetTilesInCircle(explosion_center.x, explosion_center.y, Mathf.CeilToInt(outer_radius * TerrainDefines.terrain_PPU), false))
        {
            float distance = (Zim.position - explosion_center).magnitude;
            int damage = 0;

            if (inner_radius > 0 && distance < inner_radius * TerrainDefines.terrain_PPU)
                damage = inner_damage;
            else if (middle_radius > 0 && distance < middle_radius * TerrainDefines.terrain_PPU)
                damage = middle_damage;
            else if (outer_radius>0)
                damage = outer_damage;

            TakeDamage(Zim, damage, true);
        }
        bool needUpdate = nTiles > 0 && nTiles < 1000;
        StartUpdateTiles(needUpdate);
    }

    public void StainTiles(Vector2 world_position, float radius, Color32 color)
    {
        Vector2Int explosion_center = worldtotilePosition(world_position);
        foreach (Plixel Zim in GetTilesInCircle(explosion_center.x, explosion_center.y, Mathf.CeilToInt(radius * TerrainDefines.terrain_PPU), false))
        {
            if (!Zim.stain)
            {
                Zim.ChangeColor(color);
                referenceTexture.SetPixel(Zim.position.x, Zim.position.y, Zim.getColor());
                Zim.stain = true;
            }
        }
        referenceTexture.Apply();
    }

    public void TakeDamage(Plixel tile, int damage, bool revise)
    {

        if (!tile.IsSolid() || tile.getIndestructable())
        {
            return;
        }

        if ((tile.position.x >= 0 && tile.position.y >= 0 && tile.position.x < terrain.GetLength(1) && tile.position.y < terrain.GetLength(0)))
        {
            tile.Damage(damage);
            if (revise)
            {
                tile_revision.Add(tile);
            }
        }
    }

    public void DestroyTiles(Plixel[] tiles, bool revises)
    {
        if (tiles.Length > 0)
        {
            foreach (Plixel tile in tiles)
            {
                TakeDamage(tile, 200, revises);
            }

            if (revises)
            {
                StartUpdateTiles(true);
            }
        }
    }
    public override void ApplyForce(Vector2 force, Vector2 center)
    {
         rigidbody.AddForceAtPosition(force, center);
        WorldController.active.MobsInMotion.Add(this);
        Debug.Log("[PlixelMapMob] Apply " + force + " force to " + name + " at point " + center);

    }
    public void DrawTexture()
    {
        Debug.Log("[entityTerrain] Initialize texture of " + name);
        referenceTexture = new Texture2D(terrain.GetLength(1), terrain.GetLength(0));
        List<Color> pixels = new List<Color>();
        for (int iY = 0; iY < terrain.GetLength(0); iY++)
        {
            for (int iX = 0; iX < terrain.GetLength(1); iX++)
            {
                if (terrain[iY, iX] != null)
                {
                    pixels.Add(terrain[iY, iX].getColor());
                }
                else
                {
                    pixels.Add(Color.clear);
                }
            }
        }
        referenceTexture.SetPixels(pixels.ToArray(), referenceTexture.loadedMipmapLevel);
        referenceTexture.filterMode = FilterMode.Point;
        referenceTexture.Apply();
    }

    public void StartUpdateTiles(bool update_physix)
    {
        if (RevisionCoroutine == null)
        {
            RevisionCoroutine = StartCoroutine(HandleTileChanges(update_physix));
            WorldController.active.StartCoroutine(WorldController.active.PauseForTerrainToLoad());
        }
    }
    public IEnumerator HandleTileChanges(bool update_physix)
    {
        float starttime = Time.realtimeSinceStartup;
        float ram = 0;
        int cap = TerrainDefines.terrain_ram_size;

        Debug.Log("[entityTerrain] " + name + " revises chunks...");
        List<PlixelMapMob> chunks = new List<PlixelMapMob>();
        List<Plixel[]> families = new List<Plixel[]>();
        search_index++;

        if (tile_revision.Count == 0)
        {
            Debug.Log("[entityTerrain] " + name + " has no tiles to revise..");
            if (RevisionCoroutine != null)
            {
                StopCoroutine(RevisionCoroutine);
                RevisionCoroutine = null;
            }
        }

        if (update_physix)
        {
            Debug.Log("[entityTerrain] " + name + " checks " + tile_revision.Count + " tiles with search i " + search_index);
            for (int iTile = 0; iTile < tile_revision.Count; iTile++)
            {
                Plixel Zim = tile_revision[iTile];
                if (Zim != null && Zim.search_index < search_index && Zim.IsSolid())
                {
                    Zim.search_index = search_index;
                    families.Add(Zim.getFamily());

                }
            }
            yield return new WaitForEndOfFrame();
            if (families.Count < 1)
            {
                Debug.Log("[entityTerrain] fail chunkrevision on " + name);
                StopCoroutine(RevisionCoroutine);
                RevisionCoroutine = null;
            }

            families.Sort((Plixel[] A, Plixel[] B) => { return B.Length.CompareTo(A.Length); });

            Debug.Log("[entityTerrain] " + name + " try break into pieces");
            List<Plixel> temptiles = new List<Plixel>();
            for (int i = 1; i < families.Count; i++)
            {
                Plixel[] family = families[i];
                PlixelMapMob derbis = BreakChunk(family);
                derbis.enabled = true;
                if (derbis.nTiles > 0)
                {
                    chunks.Add(derbis);
                }
                temptiles.AddRange(family);
                ram += 2;
                if (ram > cap)
                {
                    ram = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();
            DestroyTiles(temptiles.ToArray(), false);

        }
        yield return new WaitForEndOfFrame();

        foreach (Plixel Zim in tile_revision)
        {
            referenceTexture.SetPixel(Zim.position.x, Zim.position.y, Zim.getColor());
            if (Zim.has_changed)
            {
                foreach (PlixelChunk chunk in Zim.getRelevantChunks())
                {
                    if (chunk != null)
                    {
                        chunk.revised = false;
                        chunk_updated.Add(chunk);
                    }
                }
                Zim.has_changed = false;

                ram++;
                if (ram > cap)
                {
                    ram = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        tile_revision.Clear();
        referenceTexture.Apply();
        yield return new WaitForEndOfFrame();

        //if (!visuals_only)
        {
            Debug.Log("[entityTerrain] " + name + " tryupdate collision state");
            consideredforce = Vector4.zero;
            nTiles = 0;
            solid = false;
            foreach (Plixel tile in terrain)
            {
                if (tile != null && tile.IsSolid())
                {
                    nTiles++;
                    if (tile.getFrozen())
                    {
                        solid = true;
                    }
                }
            }
            if (nTiles == 0)
            {
                Kill();
            }
            else
            {
                yield return new WaitForEndOfFrame();
                rigidbody.bodyType = solid ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
                if (chunk_updated.Count > 0)
                {
                    Debug.Log("[entityTerrain] Revise Chunk Colliders");
                    foreach (PlixelChunk Zim in chunk_updated)
                    {
                        if (!Zim.revised)
                        {
                            Zim.Revise();
                        }
                    }
                    chunk_updated.Clear();
                }

            }
        }
        StopCoroutine(RevisionCoroutine);
        RevisionCoroutine = null;
        Debug.Log("[entityTerrain] " + name + " required " + (Time.realtimeSinceStartup - starttime) + " to complete coroutine");
    }

    public override void HandleShockwave(Vector2 center, Vector2 dir, float force_delta, float force, float damage)
    {
        consideredforce = new Vector4(rigidbody.centerOfMass.x, rigidbody.centerOfMass.y, force, damage);
        base.HandleShockwave(center, dir, force_delta, force, damage);
    }

    public List<Vector2> GetValidSpawnLocations(Vector2 playerSize)
    {
        List<Plixel> possible = new List<Plixel>();

        List<Vector2> SpawnLocs = new List<Vector2>();
         Vector2Int SearchSize = Vector2Int.CeilToInt(playerSize * TerrainDefines.terrain_PPU);

         foreach (Plixel Zim in terrain)
         {
             if (Zim!=null && Zim.CanSpawnEntity())
             { possible.Add(Zim); }
         }

         foreach (Plixel Zim in possible)
         {
             bool valid = true;

             for (Vector2 vRect = Vector2.zero; valid && (vRect.y <= SearchSize.y);)
             {
                 Vector2Int center = Vector2Int.RoundToInt(Zim.position + Vector2.up + vRect - Vector2.right * SearchSize.x / 2f);

                 Plixel Gir = GetTileAt(center.x, center.y, false);
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
                 SpawnLocs.Add(tiletoworldPosition(Zim.position) + (Vector2.up  + Vector2.right / 2 )/ TerrainDefines.terrain_PPU);
             }
         }
        return SpawnLocs;
    }
    public Vector2 GetWorldSize()
    { return new Vector2(terrain.GetLength(1), terrain.GetLength(0)); }

    public bool GetNBounds()
    {
        foreach (Plixel tile in GetTilesInRect(0, 0, terrain.GetLength(1), 1, false))
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
        foreach (Plixel tile in GetTilesInRect(0, terrain.GetLength(0) - 1, terrain.GetLength(1), 1, false))
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
        foreach (Plixel tile in GetTilesInRect(terrain.GetLength(1) - 1, 0, 1, terrain.GetLength(0), false))
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
        foreach (Plixel tile in GetTilesInRect(0, 0, 1, terrain.GetLength(0), false))
        {
            if (tile.IsSolid())
            {
                return true;
            }
        }
        return false;
    }
    public void FreezeTiles(Plixel[] Tiles)
    {
        foreach (Plixel t in Tiles)
        {
            t.SetFrozen();
        }
    }
}