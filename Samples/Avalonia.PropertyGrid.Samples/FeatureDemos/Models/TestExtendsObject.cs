﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Platform;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Avalonia.PropertyGrid.Samples.FeatureDemos.Models
{
    public struct SVector3
    {
        public float x, y, z;

        public readonly override string ToString() => $"{x:0.0}, {y:0.0}, {z:0.0}";
    }

    public class TestExtendsObject : MiniReactiveObject
    {
        [Category("Struct")]
        public Vector3 vec3Object { get; set; } = new();

        [Category("Struct")]
        public SVector3 vec3Struct { get; set; }

        [Category("Struct")]
        public BindingList<SVector3> vec3BindingList { get; set; } =
        [
            new(){ x = 7.8f, y = 3.14f, z = 0.0f }
        ];

        [Category("SelectableList")]
        public SelectableList<CountryInfo> Countries { get; set; }

        [Category("Boolean")]
        public bool toggleAble { get; set; } = true;

        [Category("Boolean")]
        public bool disableAble { get; set; } = false;

        [Category("Boolean")]
        public bool? threeState { get; set; }

        [Category("Boolean")]
        [ReadOnly(true)]
        public bool readonlyBoolean { get; set; }

        [Category("Boolean")]
        public static bool readonlyBoolean2 => true;

        [Category("Boolean")]
        public bool customLabel { get; set; }
        
        [Category("Numeric")]
        [Range(0, 1024)]
        [PropertyOperationVisibility(PropertyOperationVisibility.Visible)]
        public int NoCustomOperationNumber { get; set; }
        
        [Category("Numeric")]
        [Range(0, 1024)]
        [PropertyOperationVisibility(PropertyOperationVisibility.Visible)]
        public int CustomOperationMenuNumber { get; set; }
        
        [Category("Numeric")]
        [Range(0, 1024)]
        [PropertyOperationVisibility(PropertyOperationVisibility.Visible)]
        public int CustomOperationControlNumber { get; set; }

        public TestExtendsObject()
        {
            List<CountryInfo> list = [];

            var assets = AssetLoader.GetAssets(new Uri($"avares://{GetType().Assembly.GetName().Name}/Assets/country-flags"), null);
            foreach (var asset in assets)
            {
                list.Add(new CountryInfo(asset));
            }

            Countries = new SelectableList<CountryInfo>(list, list.Find(x => x.Code == "cn") ?? list.FirstOrDefault()!);
        }
    }

    public class CountryInfo
    {
        public Media.IImage Flag { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public CountryInfo(Uri asset)
        {
            Name = Code = Path.GetFileNameWithoutExtension(asset.LocalPath);

            try
            {
                var regionInfo = new RegionInfo(Code);

                Name = regionInfo.DisplayName;
            }
            catch
            {
                // ignored
            }

            using var stream = AssetLoader.Open(asset);
            Flag = new Media.Imaging.Bitmap(stream);
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (obj is CountryInfo ci)
            {
                return Code == ci.Code;
            }

            return false;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => Code.GetHashCode();
    }
}
