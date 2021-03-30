using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Common.Constant
{
    public enum EnumStatusRequest
    {
        Pending = 1,
        Accept = 2,
        Reject = 3,
        Shipping = 4,
        Finish = 5,
    }
    public enum EnumStatusShare
    {
        Share = 1,
        Exchange = 2,
        ReadOnly = 3
    }
    public enum EnumFriendRequest
    {
        Pending = 1,
        Accept = 2,
        Reject = 3
    }
}
