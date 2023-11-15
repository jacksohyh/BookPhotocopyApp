using System;
using System.Collections.Generic;
using System.Text;

namespace BookPhotocopyApp
{
    public interface IEnvironmentHelper
    {
        string GetExternalStorageDirectory();
        string checkManageFilePermission();
    }
}
