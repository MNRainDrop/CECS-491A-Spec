﻿namespace TeamSpecs.RideAlong.SecurityLibrary.Model
{
    public class AuthUserModel
    {
        //UID
        public long UID { get; set; }
        //Username
        public string? userName { get; set; }
        //Salt
        public byte[] salt { get; set; }
        //UserHash
        public string? userHash { get; set; }
        public AuthUserModel()
        {
            UID = 0;
            userName = null;
            salt = new byte[32];
            userHash = null;
        }
    }
}
