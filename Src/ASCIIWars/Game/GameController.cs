//
//  Copyright (c) 2016  FederationOfCoders.org
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

namespace ASCIIWars.Game {
    public class GameController {
        const string MAIN_SITUATION_NAME = "onEnter";

        public SituationContainer situations;
        public ItemContainer items;
        public Player player = new Player();

        Situation currentSituation;

        public GameController(SituationContainer situations, ItemContainer items) {
            this.situations = situations;
            this.items = items;
            currentSituation = situations.GetSituation(MAIN_SITUATION_NAME);
        }

        public void Start() {
            while (true) {
                try {
                    SituationController controller = SituationControllersRegistry.Get(currentSituation.type);
                    currentSituation = controller.HandleSituation(currentSituation, this);
                } catch (GameOverException) {
                    break;
                }
            }
        }
    }
}
