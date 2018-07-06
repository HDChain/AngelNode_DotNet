using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using log4net;

namespace PartnerNode.Models
{
    public class FetchUserDataTask : Singleton<FetchUserDataTask>
    {
        private Timer _timer = new Timer(10000);
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FetchUserDataTask));

        public void Start() {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
            _timer.Dispose();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) {
            _timer.Stop();

            Task.Factory.StartNew(() => {
                SyncData();
                _timer.Start();
            });

        }

        private void SyncData() {
            DbHelper.Instance.SyncUserData();
        }
    }
}
