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
}