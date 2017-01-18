using System.Collections.Generic;
using UnityEngine;
using Config;
using Config.Datatypes;

namespace Traits
{
    public class HasSprites : Trait
    {
        public string startsAs;
        public Dictionary<string, SpriteData> sprites;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var hasSpritesImplementer = go.AddComponent<Implementers.Display.HasSprites>();
            hasSpritesImplementer.startsAs = startsAs;
            hasSpritesImplementer.sprites = new Dictionary<string, Sprite>();
            foreach (KeyValuePair<string, SpriteData> spriteData in sprites)
            {
                SpriteMap spriteMap = config.spriteMaps[spriteData.Value.spriteMap];
                Sprite sprite = Sprite.Create(spriteMap.texture, spriteMap.CellRectangle(spriteData.Value.cellCoord), new Vector2(0.5f, 0.5f), spriteMap.cellSize.x);
                hasSpritesImplementer.sprites.Add(spriteData.Key, sprite);
            }

            var spriteRenderer = go.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = hasSpritesImplementer.sprites[hasSpritesImplementer.startsAs];
        }
    }
}
