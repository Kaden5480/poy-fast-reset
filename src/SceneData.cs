using System.Collections.Generic;
using UnityEngine;

namespace FastReset {
    public class SceneData {
        public Vector3 position    { get; }
        public Quaternion rotation { get; }
        public float rotationY     { get; }

        public SceneData(Vector3 position, Quaternion rotation, float rotationY) {
            this.position = position;
            this.rotation = rotation;
            this.rotationY = rotationY;
        }
    }

    public class Scenes {
        private static Dictionary<string, SceneData> scenes = new Dictionary<string, SceneData> {
        // Fundamentals
        { "Peak_1_GreenhornNEW",       new SceneData(new Vector3(3.042807f, 23.69314f, -8.716743f),  new Quaternion(0f, -0.307469f, 0f, 0.9515581f),   -20.2f)     },
        { "Peak_2_PaltryNEW",          new SceneData(new Vector3(-1.869263f, 5.244058f, -7.524767f), new Quaternion(0f, 0.06117336f, 0f, 0.9981272f),  -16.02069f) },
        { "Peak_3_OldMill",            new SceneData(new Vector3(-3.82758f, 4.877324f, -2.864442f),  new Quaternion(0f, 0.3662636f, 0f, -0.9305112f),  -10.10006f) },
        { "Peak_3_GrayGullyNEW",       new SceneData(new Vector3(-4.413444f, 27.74787f, -5.642469f), new Quaternion(0f, 0.02735011f, 0f, 0.999626f),   -24.20517f) },
        { "Peak_LighthouseNew",        new SceneData(new Vector3(17.2675f, 30.41351f, -55.71387f),   new Quaternion(0f, -0.6833417f, 0f, 0.7300987f),  -12.8862f)  },
        { "Peak_4_OldManOfSjorNEW",    new SceneData(new Vector3(-23.14834f, 7.507897f, 2.246203f),  new Quaternion(0f, 0.6991959f, 0f, 0.7149301f),   -21.59312f) },
        { "Peak_5_GiantsShelfNEW",     new SceneData(new Vector3(-2.097759f, 4.981291f, -14.74269f), new Quaternion(0f, 0.9942121f, 0f, -0.1074356f),  -9.751725f) },
        { "Peak_8_EvergreensEndNEW",   new SceneData(new Vector3(1.655918f, 3.101632f, -10.14645f),  new Quaternion(0f, 0.09159087f, 0f, 0.9957967f),  -20.72238f) },
        { "Peak_9_TheTwinsNEW",        new SceneData(new Vector3(10.51208f, 21.25717f, 10.82098f),   new Quaternion(0f, 0.1152206f, 0f, 0.99334f),     -1.741378f) },
        { "Peak_6_OldGroveSkelf",      new SceneData(new Vector3(-4.498724f, 7.621494f, -3.306153f), new Quaternion(0f, -0.2883095f, 0f, -0.9575373f), -28.73273f) },
        { "Peak_7_HangmansLeapNEW",    new SceneData(new Vector3(13.89081f, 41.87902f, 43.30726f),   new Quaternion(0f, 0.4506551f, 0f, 0.8926982f),   0f)         },
        { "Peak_13_LandsEndNEW",       new SceneData(new Vector3(-32.20438f, 1.670826f, 33.77732f),  new Quaternion(0f, 0.7088559f, 0f, 0.7053534f),   6.094826f)  },
        { "Peak_19_OldLangr",          new SceneData(new Vector3(126.5563f, 0.239604f, 8.958969f),   new Quaternion(0f, -0.5180165f, 0f, 0.8553706f),  -5.050012f) },
        { "Peak_14_Cavern",            new SceneData(new Vector3(10.87715f, 6.696132f, -10.11349f),  new Quaternion(0f, -0.1311941f, 0f, 0.9913567f),  -18.80689f) },
        { "Peak_16_ThreeSeaStacks",    new SceneData(new Vector3(-25.50616f, 10.525f, 1.173227f),    new Quaternion(0f, 0.4985752f, 0f, 0.8668464f),   0.5224143f) },
        { "Peak_10_WaltersCragNEW",    new SceneData(new Vector3(8.656415f, 6.210258f, -6.687114f),  new Quaternion(0f, -0.4241548f, 0f, 0.9055897f),  -19.50346f) },
        { "Peak_15_TheGreatCrevice",   new SceneData(new Vector3(-35.8814f, 100.5917f, -12.65881f),  new Quaternion(0f, 0.8442389f, 0f, 0.5359672f),   0f)         },
        { "Peak_17_RainyPeak",         new SceneData(new Vector3(-37.20079f, 24.24101f, 116.8735f),  new Quaternion(0f, -0.5554339f, 0f, 0.8315607f),  -28.38451f) },
        { "Peak_18_FallingBoulders",   new SceneData(new Vector3(-51.89983f, 23.63846f, 111.1374f),  new Quaternion(0f, -0.7778122f, 0f, 0.6284968f), -14.80172f)  },
        { "Peak_11_WutheringCrestNEW", new SceneData(new Vector3(-38.50421f, 24.97796f, 126.0084f),  new Quaternion(0f, -0.4397683f, 0f, 0.8981113f), -34.30518f)  },

        // Intermediate
        { "Boulder_1_OldWalkersBoulder", new SceneData(new Vector3(-0.5937033f, 0.8671017f, -11.02829f), new Quaternion(0f, 0.008790329f, 0f, 0.9999614f), -18.45862f) },
        { "Boulder_2_JotunnsThumb",      new SceneData(new Vector3(-3.05922f, 0.6709571f, -11.12061f),   new Quaternion(0f, -0.6700152f, 0f, 0.7423475f),  -22.4638f)  },
        { "Boulder_3_OldSkerry",         new SceneData(new Vector3(1.302205f, -0.8605963f, -9.193541f),  new Quaternion(0f, -0.5280481f, 0f, 0.8492144f),  -6.79138f)  },
        { "Boulder_4_TheHamarrStone",    new SceneData(new Vector3(-2.250599f, 0.2034293f, -10.8472f),   new Quaternion(0f, -0.1453226f, 0f, 0.9893844f),  -28.03619f) },
        { "Boulder_5_GiantsNose",        new SceneData(new Vector3(-1.776018f, 0.3485768f, -11.09012f),  new Quaternion(0f, 0.2974505f, 0f, -0.9547373f),  -10.44828f) },
        { "Boulder_6_WaltersBoulder",    new SceneData(new Vector3(-3.023388f, 0.6872616f, -10.50469f),  new Quaternion(0f, -0.1191952f, 0f, 0.9928709f),  -21.07068f) },
        { "Boulder_7_SunderedSons",      new SceneData(new Vector3(-3.696907f, 0.5700453f, -16.31384f),  new Quaternion(0f, -0.2064342f, 0f, 0.9784606f),  -1.21897f)  },
        { "Boulder_8_OldWealdsBoulder",  new SceneData(new Vector3(-4.702875f, 0.5661743f, -11.97508f),  new Quaternion(0f, 0.0192418f, 0f, 0.9998149f),   -21.94137f) },
        { "Boulder_9_LeaningSpire",      new SceneData(new Vector3(-3.1933f, -0.5787476f, -8.494281f),   new Quaternion(0f, 0.997482f, 0f, -0.07092024f),  -28.9069f)  },
        { "Boulder_10_Cromlech",         new SceneData(new Vector3(2.191084f, -2.050335f, -23.24788f),   new Quaternion(0f, -0.3035574f, 0f, 0.9528132f),  -6.094835f) },

        // Advanced
        { "Tind_1_WalkersPillar", new SceneData(new Vector3(14.35591f, 19.55577f, 6.311682f),  new Quaternion(0f, 0.005470063f, 0f, 0.9999851f), -13.7569f)  },
        { "Tind_3_GreatGaol",     new SceneData(new Vector3(17.06722f, 18.51238f, 4.52352f),   new Quaternion(0f, 0.1470069f, 0f, 0.9891356f),   -25.59829f) },
        { "Tind_2_Eldenhorn",     new SceneData(new Vector3(-6.245132f, 12.54477f, 23.92718f), new Quaternion(0f, 0.5611095f, 0f, 0.8277416f),   -43.88274f) },
        { "Tind_4_StHaelga",      new SceneData(new Vector3(3.977189f, 5.820568f, 38.09805f),  new Quaternion(0f, 0.1474113f, 0f, 0.9890754f),   -28.21035f) },
        { "Tind_5_YmirsShadow",   new SceneData(new Vector3(4.78034f, 3.56019f, 51.62979f),    new Quaternion(0f, 0.1341973f, 0f, 0.9909546f),   -33.78276f) },

        // Expert
        { "Category4_1_FrozenWaterfall", new SceneData(new Vector3(-3.322456f, -64.39492f, 28.99156f), new Quaternion(0f, 0.5213398f, 0f, -0.8533493f), -0.522419f) },
        { "Category4_2_SolemnTempest",   new SceneData(new Vector3(-168.6246f, -154.064f, -16.92793f), new Quaternion(0f, 0.6488209f, 0f, -0.7609411f), -10.1f)     },

        // Essentials
        { "Alps_1_TrainingTower",    new SceneData(new Vector3(4.030309f, -11.42724f, -7.280912f),  new Quaternion(0f, 0.9999999f, 0f, -0.0004620027f), -6.79138f)  },
        { "Alps_2_BalancingBoulder", new SceneData(new Vector3(-138.0107f, -11.20987f, 145.7411f),  new Quaternion(0f, 0.7403967f, 0f, 0.6721702f),     -11.84138f) },
        { "Alps_3_SeaArch",          new SceneData(new Vector3(-1.980678f, -9.04775f, -14.08282f),  new Quaternion(0f, -0.181503f, 0f, -0.9833905f),    -12.01553f) },
        { "Alps_4_SunfullSpire",     new SceneData(new Vector3(-4.171906f, 0.4253815f, -22.16451f), new Quaternion(0f, -0.9413403f, 0f, -0.337459f),    -30.64828f) },
        { "Alps_5_Tree",             new SceneData(new Vector3(-6.927839f, -165.5657f, -36.42739f), new Quaternion(0f, 0.1788157f, 0f, 0.9838827f),     -19.85172f) },
        { "Alps_6_Treppenwald",      new SceneData(new Vector3(-20.61665f, 244.7859f, 37.79231f),   new Quaternion(0f, 0.7992062f, 0f, -0.6010569f),    -17.76208f) },
        { "Alps_7_Castle",           new SceneData(new Vector3(-75.53399f, -41.76396f, 35.25882f),  new Quaternion(0f, 0.7252581f, 0f, 0.6884771f),     -8.358623f) },
        { "Alps_8_SeaSideTraining",  new SceneData(new Vector3(-6.378055f, -8.369205f, -15.42508f), new Quaternion(0f, 0.003096205f, 0f, -0.9999952f),  16.36897f)  },
        { "Alps_9_IvoryGranites",    new SceneData(new Vector3(-2.704333f, -7.369945f, 5.020199f),  new Quaternion(0f, 0.6307028f, 0f, -0.7760245f),    -26.29483f) },
        { "Alps_10_Rekkja",          new SceneData(new Vector3(133.0403f, -52.37831f, -18.17772f),  new Quaternion(0f, 0.7272176f, 0f, 0.686407f),      -18.98103f) },
        { "Alps_11_Quietude",        new SceneData(new Vector3(-11.4681f, -5.832056f, -21.74628f),  new Quaternion(0f, 0.4872071f, 0f, -0.8732864f),    -14.45345f) },
        { "Alps_12_Overlook",        new SceneData(new Vector3(-1.999746f, 27.66229f, 40.88813f),   new Quaternion(0f, 0.2678598f, 0f, 0.9634579f),     -8.010345f) },

        // Alpine Greats
        { "Alps2_1_Waterfall",      new SceneData(new Vector3(-503.7092f, -24.25526f, 27.3785f),    new Quaternion(0f, 0.881979f, 0f, -0.4712887f),  -17.06551f) },
        { "Alps2_2_Dam",            new SceneData(new Vector3(-17.19395f, -1.691761f, -14.8833f),   new Quaternion(0f, 0.9218983f, 0f, -0.387432f),  -17.06552f) },
        { "Alps2_3_Dunderhorn",     new SceneData(new Vector3(-164.1781f, 195.3665f, 109.9276f),    new Quaternion(0f, 0.7879508f, 0f, 0.6157383f),  -25.24998f) },
        { "Alps2_4_ElfenbenSpires", new SceneData(new Vector3(-0.8677062f, -15.04741f, -18.65361f), new Quaternion(0f, 0.2279034f, 0f, -0.9736838f), -16.5431f)  },
        { "Alps2_5_WelkinPass",     new SceneData(new Vector3(-98.14774f, 61.91707f, -32.58269f),   new Quaternion(0f, 0.2183849f, 0f, -0.9758627f), -17.4138f)  },

        // Arctic and Arduous
        { "Alps3_1_SeigrCraeg",    new SceneData(new Vector3(18.02618f, -14.82672f, 84.98164f),   new Quaternion(0f, 0.7165545f, 0f, -0.6975311f),  -34.65331f) },
        { "Alps3_2_UllrsGate",     new SceneData(new Vector3(-13.09935f, -6.217767f, -27.53476f), new Quaternion(0f, 0.07400473f, 0f, -0.9972579f), 29.42927f)  },
        { "Alps3_3_GreatSilf",     new SceneData(new Vector3(10.3781f, -16.46482f, 30.01005f),    new Quaternion(0f, 0.5475876f, 0f, 0.8367484f),   -33.60863f) },
        { "Alps3_4_ToweringVisir", new SceneData(new Vector3(-17.04677f, -306.7072f, 8.217235f),  new Quaternion(0f, -0.2077558f, 0f, -0.9781808f), -47.01725f) },
        { "Alps3_5_EldrisWall",    new SceneData(new Vector3(91.30836f, 29.11353f, 377.7364f),    new Quaternion(0f, 0.793372f, 0f, 0.6087371f),    -24.90172f) },
        { "Alps3_6_MountMhorgorm", new SceneData(new Vector3(87.40735f, -592.0008f, -61.40174f),  new Quaternion(0f, -0.6875257f, 0f, 0.72616f),    -23.33447f) },
        };

        public static SceneData GetScene(string name) {
            SceneData data = null;

            if (name == null) {
                return null;
            }

            if (scenes.TryGetValue(name, out data) == false) {
                return null;
            }

            return data;
        }
    }
}
