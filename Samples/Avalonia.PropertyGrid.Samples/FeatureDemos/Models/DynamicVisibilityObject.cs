﻿using System;
using System.ComponentModel;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using PropertyModels.Extensions;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Models
{
    public class DynamicVisibilityObject : ReactiveObject
    {
        [ConditionTarget]
        public bool IsShowPath { get; set; } = true;

        [PropertyVisibilityCondition(nameof(IsShowPath), true)]
        [PathBrowsable(Filters = "Image Files(*.jpg;*.png;*.bmp;*.tag)|*.jpg;*.png;*.bmp;*.tag")]
        public string Path { get; set; } = string.Empty;

        [ConditionTarget]
        public PlatformID Platform { get; set; } = PlatformID.Win32NT;

        [PropertyVisibilityCondition(nameof(Platform), PlatformID.Unix)]
        [ConditionTarget]
        public string UnixVersion { get; set; } = string.Empty;

        // show more complex conditions...
        [Browsable(false)]
        [DependsOnProperty(nameof(IsShowPath), nameof(Platform), nameof(UnixVersion))]
        [ConditionTarget]
        public bool IsShowUnixLoginInfo => IsShowPath && Platform == PlatformID.Unix && UnixVersion.IsNotNullOrEmpty();

        [PropertyVisibilityCondition(nameof(IsShowUnixLoginInfo), true)]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        // ReSharper disable once InconsistentNaming
        public LoginInfo unixLogInInfo { get; set; } = new();
    }
}
