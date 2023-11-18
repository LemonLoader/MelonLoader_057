namespace MonkiiLoader
{
    public static class MonkiiEvents
    {
        /// <summary>
        /// Called after all MonkiiPlugins are initialized.
        /// </summary>
        public readonly static MonkiiEvent OnPreInitialization = new(true);

        /// <summary>
        /// Called after Game Initialization, before OnApplicationStart and before Assembly Generation on Il2Cpp games.
        /// </summary>
        public readonly static MonkiiEvent OnApplicationEarlyStart = new(true);

        /// <summary>
        /// Called after all MonkiiMods are initialized and before the right Support Module is loaded.
        /// </summary>
        public readonly static MonkiiEvent OnPreSupportModule = new(true);

        /// <summary>
        /// Called after all MonkiiLoader components are fully initialized (including all MonkiiMods).
        /// <para>Don't use this event to initialize your Monkiis anymore! Instead, override <see cref="MonkiiBase.OnInitializeMonkii"/>.</para>
        /// </summary>
        public readonly static MonkiiEvent OnApplicationStart = new(true);

        /// <summary>
        /// Called when the first 'Start' Unity Messages are invoked.
        /// </summary>
        public readonly static MonkiiEvent OnApplicationLateStart = new(true);

        /// <summary>
        /// Called before the Application is closed. It is not possible to prevent the game from closing at this point.
        /// </summary>
        public readonly static MonkiiEvent OnApplicationDefiniteQuit = new(true);

        /// <summary>
        /// Called on a quit request. It is possible to abort the request in this callback.
        /// </summary>
        public readonly static MonkiiEvent OnApplicationQuit = new();

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public readonly static MonkiiEvent OnUpdate = new();

        /// <summary>
        /// Called every 0.02 seconds, unless Time.fixedDeltaTime has a different Value. It is recommended to do all important Physics calculations inside this Callback.
        /// </summary>
        public readonly static MonkiiEvent OnFixedUpdate = new();

        /// <summary>
        /// Called once per frame, after <see cref="OnUpdate"/>.
        /// </summary>
        public readonly static MonkiiEvent OnLateUpdate = new();

        /// <summary>
        /// Called at every IMGUI event. Only use this for drawing IMGUI Elements.
        /// </summary>
        public readonly static MonkiiEvent OnGUI = new();

        /// <summary>
        /// Called when a new Scene is loaded.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MonkiiEvent<int, string> OnSceneWasLoaded = new();

        /// <summary>
        /// Called once a Scene is initialized.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MonkiiEvent<int, string> OnSceneWasInitialized = new();

        /// <summary>
        /// Called once a Scene unloads.
        /// <para>
        /// Arguments:
        /// <br><see cref="int"/>: Build Index of the Scene.</br>
        /// <br><see cref="string"/>: Name of the Scene.</br>
        /// </para>
        /// </summary>
        public readonly static MonkiiEvent<int, string> OnSceneWasUnloaded = new();

        /// <summary>
        /// Called before MonkiiMods are loaded from the Mods folder.
        /// </summary>
        public readonly static MonkiiEvent OnPreModsLoaded = new(true);

        internal readonly static MonkiiEvent MonkiiHarmonyEarlyInit = new(true);
        internal readonly static MonkiiEvent MonkiiHarmonyInit = new(true);
    }
}
