﻿using System;

namespace MonkiiLoader
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class VerifyLoaderBuildAttribute : Attribute
    {
        /// <summary>
        /// Build HashCode of MonkiiLoader.
        /// </summary>
        public string HashCode { get; internal set; }

        public VerifyLoaderBuildAttribute(string hashcode) { HashCode = hashcode; }

        public bool IsCompatible(string hashCode)
            => string.IsNullOrEmpty(HashCode) || string.IsNullOrEmpty(hashCode) || HashCode == hashCode;
    }
}