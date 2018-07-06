namespace PartnerNode.Models {

    public class Singleton<T> where T : new() {
        private static T MInstance;
        private static readonly object sync = new object();
        protected Singleton() { }

        public static T Instance {
            get {
                if (MInstance == null) {
                    lock (sync) {
                        if (MInstance == null) {
                            MInstance = new T();
                        }
                    }
                }
                return MInstance;
            }
        }
    }
}
