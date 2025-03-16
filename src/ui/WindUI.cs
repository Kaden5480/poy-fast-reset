using UnityEngine;
using UnityEngine.UI;

namespace FastReset.UI {
    /**
     * <summary>
     * Handles the creation/management/deletion of
     * the UI for displaying the current wind state.
     * </summary>
     */
    public class WindUI : Loggable {
        // The currently active instance
        private static WindUI instance = null;

        // UI accessible anywhere
        public static GameObject ui {
            get => (instance == null) ? null : instance.uiObj;
        }

        // The root object holding the UI
        private GameObject uiObj = null;

        // The rect transform for the UI object
        private RectTransform uiRect = null;

        // The filenames for the different icons
        private const string windIconName = "wind.png";
        private const string strikeIconName = "strike.png";

        // The scoreboard icon's object
        private GameObject scoreboardObj = null;

        // Position when the scoreboard icon is active
        private Vector2 scoreboardPosition = Vector2.zero;
        // Position otherwise
        private Vector2 normalPosition = Vector2.zero;

        // The icons
        private GameObject strikeIconObj = null;

        private WindUI() {
            // Instantiate
            GameObject timeAttackObj = GameObject.Find("TIMEATTACK/TimeAttackCanvas/TimeAttackText");
            if (timeAttackObj == null) {
                LogDebug("Unable to find time attack");
                return;
            }

            Transform scoreboardTransform = timeAttackObj.transform.Find("scoreboard_image");
            if (scoreboardTransform == null) {
                LogDebug("Unable to find scoreboard transform");
                return;
            }

            // Track scoreboard object
            scoreboardObj = scoreboardTransform.gameObject;

            uiObj = GameObject.Instantiate(
                scoreboardTransform.gameObject, timeAttackObj.transform
            );

            // Remove image
            GameObject.Destroy(uiObj.GetComponent<Image>());

            uiObj.name = "Fast Reset Wind UI";

            // Disable the UI by default, let time attack
            // handle it
            uiObj.SetActive(false);

            // Adjust scale/position
            uiRect = uiObj.GetComponent<RectTransform>();

            scoreboardPosition = new Vector2(
                uiRect.anchoredPosition.x - 1,
                uiRect.anchoredPosition.y - 51
            );

            normalPosition = new Vector2(
                uiRect.anchoredPosition.x - 1,
                uiRect.anchoredPosition.y + 3
            );

            // Use the normal position by default
            uiRect.anchoredPosition = normalPosition;

            // Wind icon
            GameObject windIconObj = new GameObject("Wind Icon");
            windIconObj.transform.SetParent(uiObj.transform);
            windIconObj.transform.localPosition = Vector3.zero;

            Image windIcon = windIconObj.AddComponent<Image>();
            windIcon.material = Material.Instantiate(windIcon.material);
            windIcon.material.mainTexture = ImageLoader.LoadTexture(windIconName);

            windIconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

            // Strike icon
            strikeIconObj = new GameObject("Strike Icon");
            strikeIconObj.transform.SetParent(uiObj.transform);
            strikeIconObj.transform.localPosition = Vector3.zero;

            Image strikeIcon = strikeIconObj.AddComponent<Image>();
            strikeIcon.material = Material.Instantiate(strikeIcon.material);
            strikeIcon.material.mainTexture = ImageLoader.LoadTexture(strikeIconName);

            strikeIconObj.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

            // Initialize to show wind as enabled
            strikeIconObj.SetActive(false);

            LogDebug("Created UI");

        }

        /**
         * <summary>
         * Creates the UI if it doesn't exist already.
         * </summary>
         */
        public static void Create() {
            if (instance != null) {
                Plugin.LogDebug($"[{typeof(WindUI)}] Instance already created");
                return;
            }

            instance = new WindUI();
        }

        /**
         * <summary>
         * Destroys the UI if it exists.
         * </summary>
         */
        public static void Destroy() {
            if (instance == null) {
                Plugin.LogDebug($"[{typeof(WindUI)}] UI already destroyed");
                return;
            }

            GameObject.Destroy(instance.uiObj);
            instance = null;

            Plugin.LogDebug($"[{typeof(WindUI)}] Destroyed UI");
        }

        /**
         * <summary>
         * Determine which state to show the wind in.
         * </summary>
         * <param name="enabled">Whether the wind is enabled</param>
         */
        public static void ShowEnabled(bool enabled) {
            if (instance == null) {
                Plugin.LogDebug($"[{typeof(WindUI)}] No instance to enable/disable");
                return;
            }

            if (instance.uiObj == null) {
                instance.LogDebug("UI object not found");
                return;
            }

            if (instance.strikeIconObj == null) {
                instance.LogDebug("No strike icon found");
                return;
            }

            string enabledString = (enabled == true) ? "enabled" : "disabled";
            instance.LogDebug($"Showing the wind is currently {enabledString}");
            instance.strikeIconObj.SetActive(!enabled);
        }

        /**
         * <summary>
         * Update the position depending on whether the scoreboard is active.
         * </summary>
         */
        public static void UpdatePosition() {
            if (instance == null) {
                Plugin.LogDebug($"[{typeof(WindUI)}] No instance to update position for");
                return;
            }

            if (instance.scoreboardObj.activeSelf == true) {
                instance.uiRect.anchoredPosition = instance.scoreboardPosition;
            }
            else {
                instance.uiRect.anchoredPosition = instance.normalPosition;
            }
        }
    }
}
