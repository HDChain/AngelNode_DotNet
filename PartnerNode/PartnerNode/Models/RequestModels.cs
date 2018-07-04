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


    public class ReqLoadUserFile : BaseReq {
        public string Account { get; set; }

        public string FileKey { get; set; }

        public string ContractAddress { get; set; }

    }

    public class RespLoadUserFile : BaseResp {
        public object Ret { get; set; }
    }



    #endregion















}