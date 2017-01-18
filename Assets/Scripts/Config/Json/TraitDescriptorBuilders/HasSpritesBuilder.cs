using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Config.Datatypes;
using Traits;

namespace Config.Json.TraitDescriptorBuilders
{
    public static class HasSpritesBuilder
    {
        public static HasSprites Build (JToken attributesJson)
        {
            // Remove the sprites token and save it to add afterwards, this lets us just feed the remaining attributes into the ToObject call.
            JToken spritesToken = attributesJson["sprites"];
            spritesToken.Parent.Remove();

            HasSprites hasSpritesTrait = attributesJson.ToObject<HasSprites>();

            // Add the sprites manually, this is the bit the json parser otherwise chokes on.
            hasSpritesTrait.sprites = new Dictionary<string, SpriteData>();
            foreach (JProperty spriteToken in spritesToken.Children())
            {
                hasSpritesTrait.sprites.Add(spriteToken.Name, spriteToken.Value.ToObject<SpriteData>());
            }

            return hasSpritesTrait;
        }
    }
}
