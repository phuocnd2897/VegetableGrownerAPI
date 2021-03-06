﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VG.Model.ResponseModel
{
    public class SelectedResponseModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
    public class AccountSelectedResponseModel : SelectedResponseModel
    {
        public string Avatar { get; set; }
    }
    public class LockedAccountSelectedResponseModel: SelectedResponseModel
    {
        public DateTime? DateUnLock { get; set; }
    }
}
