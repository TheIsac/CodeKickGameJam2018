using System.Collections.Generic;

namespace _20180713._Scripts
{
    public class ArenaSizeSlider : SettingsSlider<GameStarter.ArenaSize>
    {
        public ArenaSizeSlider()
        {
            IndexToTextAndValueMap = new Dictionary<int, KeyValuePair<string, GameStarter.ArenaSize>>
            {
                {
                    1, new KeyValuePair<string, GameStarter.ArenaSize>("Small", GameStarter.ArenaSize.Small)
                },
                {
                    2, new KeyValuePair<string, GameStarter.ArenaSize>("Large", GameStarter.ArenaSize.Large)
                },
                {
                    3, new KeyValuePair<string, GameStarter.ArenaSize>("Gigantic", GameStarter.ArenaSize.Gigantic)
                }
            };
        }
    }
}