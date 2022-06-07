using UnityEngine;
using System.Collections.Generic;


public class Plixel
{

    public bool stain = false;
    public int search_index = 0;
    public PlixelMapMob parent;
    public Vector2Int position = Vector2Int.one * -1;
    public bool has_changed = false;

    protected int thoughness = 1;
    protected TerrainDefines.Element element = 0;

    protected Color tcolor;
    protected int tcollision;

    public Plixel() { }

    public Plixel(PlixelMapMob game)
    {
        parent = game;
    }

    public virtual void CloneTile(Plixel other)
    {
        ChangeElement(other.element);
    }

    public Plixel(PlixelMapMob game, int x, int y, TerrainDefines.Element telement)
    {
        parent = game;
        position = new Vector2Int(x, y);
        ChangeElement(telement);
    }

    public Plixel(PlixelMapMob game, int x, int y, Color celement, bool foreground)
    {
        parent = game;
        position = new Vector2Int(x, y);
        ChangeElement(celement);
    }

    public Plixel(PlixelMapMob game, int x, int y)
    {
        parent = game;
        position = new Vector2Int(x, y);

        tcolor = game.GetColorAt(x, y);
        ChangeElement(tcolor);
    }
    public void ChangeElement(Color col)
    {
        if (col.a < .5f)
        {
            ChangeElement(TerrainDefines.Element.nothing);
        }
        else if (col == Color.red)
        {
            ChangeElement(TerrainDefines.Element.core);
        }
        else if (col == Color.black)
        {
            ChangeElement(TerrainDefines.Element.rock);
        }
        else if (col == Color.white)
        {
            ChangeElement(TerrainDefines.Element.fertilizer);
        }
        else if (col.r == 1 && col.g == 1 && col.b == 0)
        {
            ChangeElement(TerrainDefines.Element.gold);
        }
        else
        {
            ChangeElement(TerrainDefines.Element.dirt);
        }
        if (col.a < 1f)
        {
            Kill(false);
        }
    }
    public void ChangeElement(TerrainDefines.Element nelement)
    {
        tcolor = TerrainDefines.ElementColors[(int)nelement];
        element = nelement;
        switch (nelement)
        {
            case TerrainDefines.Element.nothing:
                thoughness = 0;
                tcollision = 0;
                break;
            case TerrainDefines.Element.dirt:
                tcollision = (int)TerrainDefines.Behavior.Foreground;
                thoughness = 1;
                break;
            case TerrainDefines.Element.rock:
                tcollision = (int)TerrainDefines.Behavior.Foreground;
                thoughness = 3;
                break;
            case TerrainDefines.Element.gold:
                tcollision = (int)TerrainDefines.Behavior.Foreground;
                thoughness = 2;
                break;
            case TerrainDefines.Element.fertilizer:
                tcollision = (int)TerrainDefines.Behavior.Foreground;
                thoughness = 2;
                break;
            case TerrainDefines.Element.core:
                tcollision = (int)TerrainDefines.Behavior.Foreground | (int)TerrainDefines.Behavior.Indestructable;
                thoughness = 999;
                break;
        }
    }
    public TerrainDefines.Element GetElement()
    {
        return element;
    }

    public Plixel[] GetNeighbors(bool relevant)
    {
        return new Plixel[]
        {
            parent.GetTileAt(position.x + 1, position.y, relevant),
            parent.GetTileAt(position.x - 1, position.y, relevant),
            parent.GetTileAt(position.x, position.y + 1, relevant),
            parent.GetTileAt(position.x, position.y - 1, relevant)
        };
    }
    public bool CanSpawnEntity()
    {
        if (!IsForeGround() || parent.GetTileAt(position.x, position.y + 1, false) == null)
        {
            return false;
        }
        if (!parent.GetTileAt(position.x, position.y + 1, true).IsForeGround())
        {
            return true;
        }
        else
        if (!parent.GetTileAt(position.x, position.y + 2, true).IsForeGround())
        {
            return true;
        }
        else if(!parent.GetTileAt(position.x, position.y + 3, true).IsForeGround())
        {
            return true;
        }
        return false;
    }
    public PixelColliderSegment[] getCollision(Rect bounds, Vector2 center, bool foreground)
    {
        List<PixelColliderSegment> points = new List<PixelColliderSegment>();
        Plixel[] neighbors = GetNeighbors(false);
        center = ((Vector2)position + Vector2.one / 2f) - center;

        if (neighbors[0] == null || position.x >= bounds.xMax || !neighbors[0].IsSolid(foreground))//r
        {
            points.Add(new PixelColliderSegment(center + (Vector2.right + Vector2.up) / 2, center + (Vector2.right + Vector2.down) / 2));
        }
        if (neighbors[1] == null || position.x <= bounds.xMin || !neighbors[1].IsSolid(foreground))//l
        {
            points.Add(new PixelColliderSegment(center + (Vector2.left + Vector2.up) / 2, center + (Vector2.left + Vector2.down) / 2));
        }
        if (neighbors[2] == null || position.y >= bounds.yMax || !neighbors[2].IsSolid(foreground))//u
        {
            points.Add(new PixelColliderSegment(center + (Vector2.up + Vector2.left) / 2, center + (Vector2.up + Vector2.right) / 2));
        }
        if (neighbors[3] == null || position.y <= bounds.yMin || !neighbors[3].IsSolid(foreground))//d
        {
            points.Add(new PixelColliderSegment(center + (Vector2.down + Vector2.left) / 2, center + (Vector2.down + Vector2.right) / 2));
        }

        return points.ToArray();
    }
    public Plixel[] getFamily()
    {
        List<Plixel> family = new List<Plixel> { this };
        List<Plixel> neighbors = new List<Plixel>();

        neighbors.AddRange(GetNeighbors(false));
        for (int n = 0; n < neighbors.Count; n++)
        {
            Plixel neighbor = neighbors[n];
            if (neighbor != null && neighbor.search_index < this.search_index && neighbor.IsSolid())
            {
                neighbor.search_index = this.search_index;
                family.Add(neighbor);
                neighbors.AddRange(neighbor.GetNeighbors(false));
            }
        }

        return family.ToArray();
    }

    public PlixelChunk[] getRelevantChunks()
    {
        List<PlixelChunk> relevant = new List<PlixelChunk>()
        {
             parent.getChunkAt(parent.tiletochunkPosition(position.x, position.y))
        };
        if (position.x % TerrainDefines.terrain_chunk_size == 0)
        {
            relevant.Add(parent.getChunkAt(parent.tiletochunkPosition(position.x - TerrainDefines.terrain_chunk_size, position.y)));
        }
        else if (position.x % TerrainDefines.terrain_chunk_size == TerrainDefines.terrain_chunk_size - 1)
        {
            relevant.Add(parent.getChunkAt(parent.tiletochunkPosition(position.x + TerrainDefines.terrain_chunk_size, position.y)));
        }
        if (position.y % TerrainDefines.terrain_chunk_size == 0)
        {
            relevant.Add(parent.getChunkAt(parent.tiletochunkPosition(position.x, position.y - TerrainDefines.terrain_chunk_size)));
        }
        else if (position.y % TerrainDefines.terrain_chunk_size == TerrainDefines.terrain_chunk_size - 1)
        {
            relevant.Add(parent.getChunkAt(parent.tiletochunkPosition(position.x, position.y + TerrainDefines.terrain_chunk_size)));
        }
        return relevant.ToArray();
    }
    public Color getColor()
    {
        if (IsSolid())
        {
            if (IsBackGround())
            {
                float delta = .2f;
                Color n_c = new Color(tcolor.r * delta, tcolor.b * delta, tcolor.b * delta, tcolor.a);
                return n_c;
            }
            return tcolor;
        }
        else { return Color.clear; }
    }
    public void ChangeColor(Color color)
    {
        tcolor = color;
    }
    public virtual void Damage(int damage)
    {
        if (thoughness>0 && damage >= thoughness)
        {
            Kill(damage> thoughness);
        }
    }

    public virtual void Kill(bool permanent)
    {
        if (!getIndestructable())
        {
            if (!IsBackGround())
            {
                ResourceHarvestController.active.OnTileHarvested(this);
            }
            if (permanent )
            {
               tcollision = 0;
                thoughness = 0;
            }
            else if (IsSolid())
            {
                tcollision = (int) TerrainDefines.Behavior.Background | (getFrozen() ? (int)TerrainDefines.Behavior.Frozen : 0); 
            }            
            has_changed = true;
        }
    }

    public bool IsRelevant(bool foreground)
    {
        if (!IsSolid(foreground))
        {
            return false;
        }
        Plixel[] Neighbors = GetNeighbors(true);
        if (Neighbors[0].IsSolid(foreground) && Neighbors[1].IsSolid(foreground) && Neighbors[2].IsSolid(foreground) && Neighbors[3].IsSolid(foreground))
        {
            return false;
        }
        return true;
    }

    public bool IsSolid()
    {
        return tcollision != 0;
    }
    public bool IsSolid(bool foreground)
    {
        if (foreground)
        {
            return IsForeGround();
        }
        return IsForeGround() ||  IsBackGround();
    }

    public bool IsForeGround()
    {
        return (tcollision & (int)TerrainDefines.Behavior.Foreground ) == (int)TerrainDefines.Behavior.Foreground;
    }

    public bool IsBackGround()
    {
        return (tcollision & (int)TerrainDefines.Behavior.Background ) == (int)TerrainDefines.Behavior.Background;
    }

    public bool getFrozen()
    {
        return (tcollision & (int)TerrainDefines.Behavior.Frozen) == (int)TerrainDefines.Behavior.Frozen;
    }

    public void SetFrozen()
    {
        tcollision |= (int)TerrainDefines.Behavior.Frozen;
    }

    public bool getIndestructable()
    {
        return (tcollision & (int)TerrainDefines.Behavior.Indestructable) == (int)TerrainDefines.Behavior.Indestructable;
    }

    public void SetIndestructable()
    {
        tcollision |= (int)TerrainDefines.Behavior.Indestructable;
    }
}
