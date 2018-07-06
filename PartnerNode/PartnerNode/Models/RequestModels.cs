using System;
using System.Collections.Generic;

namespace PartnerNode.Models {
    /// <summary>
    ///     请求基类
    /// </summary>
    public class BaseReq {
    }

    /// <summary>
    ///     响应基础类
    /// </summary>
    public class BaseResp {
        public bool Result { get; set; } = true;

        public string Error { get; set; } = string.Empty;
    }

    public class ReqAddPeer : BaseReq {
        public string Peer { get; set; }
    }

    public class RespAddPeer : BaseResp {
        public object Ret { get; set; }
    }


    #region Db

    public class ReqAddUserDataAccount : BaseReq {
        public string Account { get; set; }

        public string FileKey { get; set; }

        public string ContractAddress { get; set; }
    }

    public class ReqDeleteAccount : BaseReq {
        public int Id { get; set; }
    }


    public class ReqQueryHealthPressure : BaseReq {
        public string Account { get; set; }

        public string DateStart { get; set; }

        public string DateEnd { get; set; }
    }

    public class RespQueryHealthPressure : BaseResp {



        public List<Item> Items { get; set; } = new List<Item>();

        public class Item {
            public DateTime BuildTime { get; set; }
            public int HighPressure { get; set; }
            public int LowPressure { get; set; }
            public int Pulse { get; set; }
        }
    }


    #endregion















}