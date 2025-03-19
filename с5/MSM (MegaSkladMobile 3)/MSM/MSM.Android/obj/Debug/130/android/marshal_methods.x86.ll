; ModuleID = 'obj\Debug\130\android\marshal_methods.x86.ll'
source_filename = "obj\Debug\130\android\marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android"


%struct.MonoImage = type opaque

%struct.MonoClass = type opaque

%struct.MarshalMethodsManagedClass = type {
	i32,; uint32_t token
	%struct.MonoClass*; MonoClass* klass
}

%struct.MarshalMethodName = type {
	i64,; uint64_t id
	i8*; char* name
}

%class._JNIEnv = type opaque

%class._jobject = type {
	i8; uint8_t b
}

%class._jclass = type {
	i8; uint8_t b
}

%class._jstring = type {
	i8; uint8_t b
}

%class._jthrowable = type {
	i8; uint8_t b
}

%class._jarray = type {
	i8; uint8_t b
}

%class._jobjectArray = type {
	i8; uint8_t b
}

%class._jbooleanArray = type {
	i8; uint8_t b
}

%class._jbyteArray = type {
	i8; uint8_t b
}

%class._jcharArray = type {
	i8; uint8_t b
}

%class._jshortArray = type {
	i8; uint8_t b
}

%class._jintArray = type {
	i8; uint8_t b
}

%class._jlongArray = type {
	i8; uint8_t b
}

%class._jfloatArray = type {
	i8; uint8_t b
}

%class._jdoubleArray = type {
	i8; uint8_t b
}

; assembly_image_cache
@assembly_image_cache = local_unnamed_addr global [0 x %struct.MonoImage*] zeroinitializer, align 4
; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = local_unnamed_addr constant [272 x i32] [
	i32 32687329, ; 0: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 78
	i32 34715100, ; 1: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 107
	i32 39109920, ; 2: Newtonsoft.Json.dll => 0x254c520 => 18
	i32 57263871, ; 3: Xamarin.Forms.Core.dll => 0x369c6ff => 102
	i32 95598293, ; 4: Supabase.dll => 0x5b2b6d5 => 21
	i32 101534019, ; 5: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 92
	i32 120558881, ; 6: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 92
	i32 122350210, ; 7: System.Threading.Channels.dll => 0x74aea82 => 38
	i32 162612358, ; 8: MimeMapping => 0x9b14486 => 14
	i32 165246403, ; 9: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 59
	i32 166922606, ; 10: Xamarin.Android.Support.Compat.dll => 0x9f3096e => 43
	i32 172012715, ; 11: FastAndroidCamera.dll => 0xa40b4ab => 5
	i32 182336117, ; 12: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 93
	i32 209399409, ; 13: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 57
	i32 219130465, ; 14: Xamarin.Android.Support.v4 => 0xd0faa61 => 48
	i32 220171995, ; 15: System.Diagnostics.Debug => 0xd1f8edb => 129
	i32 230216969, ; 16: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 73
	i32 230752869, ; 17: Microsoft.CSharp.dll => 0xdc10265 => 9
	i32 231814094, ; 18: System.Globalization => 0xdd133ce => 135
	i32 232815796, ; 19: System.Web.Services => 0xde07cb4 => 120
	i32 261689757, ; 20: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 62
	i32 278686392, ; 21: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 77
	i32 280482487, ; 22: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 71
	i32 318968648, ; 23: Xamarin.AndroidX.Activity.dll => 0x13031348 => 49
	i32 321597661, ; 24: System.Numerics => 0x132b30dd => 31
	i32 334355562, ; 25: ZXing.Net.Mobile.Forms.dll => 0x13eddc6a => 110
	i32 342366114, ; 26: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 75
	i32 385762202, ; 27: System.Memory.dll => 0x16fe439a => 122
	i32 389971796, ; 28: Xamarin.Android.Support.Core.UI => 0x173e7f54 => 44
	i32 441335492, ; 29: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 61
	i32 442521989, ; 30: Xamarin.Essentials => 0x1a605985 => 101
	i32 442565967, ; 31: System.Collections => 0x1a61054f => 127
	i32 450948140, ; 32: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 70
	i32 465846621, ; 33: mscorlib => 0x1bc4415d => 16
	i32 469710990, ; 34: System.dll => 0x1bff388e => 29
	i32 476646585, ; 35: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 71
	i32 485463106, ; 36: Microsoft.IdentityModel.Abstractions => 0x1cef9442 => 10
	i32 486930444, ; 37: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 82
	i32 498788369, ; 38: System.ObjectModel => 0x1dbae811 => 130
	i32 514659665, ; 39: Xamarin.Android.Support.Compat => 0x1ead1551 => 43
	i32 526420162, ; 40: System.Transactions.dll => 0x1f6088c2 => 114
	i32 545304856, ; 41: System.Runtime.Extensions => 0x2080b118 => 128
	i32 548916678, ; 42: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 8
	i32 577335427, ; 43: System.Security.Cryptography.Cng => 0x22697083 => 124
	i32 605376203, ; 44: System.IO.Compression.FileSystem => 0x24154ecb => 118
	i32 610194910, ; 45: System.Reactive.dll => 0x245ed5de => 33
	i32 627609679, ; 46: Xamarin.AndroidX.CustomView => 0x2568904f => 66
	i32 646852959, ; 47: MSM.Android => 0x268e315f => 0
	i32 662205335, ; 48: System.Text.Encodings.Web.dll => 0x27787397 => 36
	i32 663517072, ; 49: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 98
	i32 666292255, ; 50: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 54
	i32 690569205, ; 51: System.Xml.Linq.dll => 0x29293ff5 => 40
	i32 692692150, ; 52: Xamarin.Android.Support.Annotations => 0x2949a4b6 => 42
	i32 763346851, ; 53: Websocket.Client => 0x2d7fbfa3 => 41
	i32 772621961, ; 54: Supabase.Core.dll => 0x2e0d4689 => 20
	i32 775507847, ; 55: System.IO.Compression => 0x2e394f87 => 117
	i32 809851609, ; 56: System.Drawing.Common.dll => 0x30455ad9 => 116
	i32 843511501, ; 57: Xamarin.AndroidX.Print => 0x3246f6cd => 89
	i32 877678880, ; 58: System.Globalization.dll => 0x34505120 => 135
	i32 882883187, ; 59: Xamarin.Android.Support.v4.dll => 0x349fba73 => 48
	i32 902159924, ; 60: Rg.Plugins.Popup => 0x35c5de34 => 19
	i32 920281169, ; 61: Supabase.Functions => 0x36da6051 => 22
	i32 928116545, ; 62: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 107
	i32 954320159, ; 63: ZXing.Net.Mobile.Forms => 0x38e1c51f => 110
	i32 955402788, ; 64: Newtonsoft.Json => 0x38f24a24 => 18
	i32 958213972, ; 65: Xamarin.Android.Support.Media.Compat => 0x391d2f54 => 47
	i32 967690846, ; 66: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 75
	i32 974778368, ; 67: FormsViewGroup.dll => 0x3a19f000 => 6
	i32 992768348, ; 68: System.Collections.dll => 0x3b2c715c => 127
	i32 1012816738, ; 69: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 91
	i32 1035644815, ; 70: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 53
	i32 1042160112, ; 71: Xamarin.Forms.Platform.dll => 0x3e1e19f0 => 104
	i32 1052210849, ; 72: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 79
	i32 1089187994, ; 73: Websocket.Client.dll => 0x40ebb09a => 41
	i32 1098259244, ; 74: System => 0x41761b2c => 29
	i32 1134191450, ; 75: ZXingNetMobile.dll => 0x439a635a => 112
	i32 1175144683, ; 76: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 96
	i32 1178241025, ; 77: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 86
	i32 1204270330, ; 78: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 54
	i32 1216849306, ; 79: Supabase.Realtime.dll => 0x4887a59a => 25
	i32 1219540809, ; 80: Supabase.Functions.dll => 0x48b0b749 => 22
	i32 1267360935, ; 81: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 97
	i32 1293217323, ; 82: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 68
	i32 1336984896, ; 83: Supabase.Core => 0x4fb0c540 => 20
	i32 1364015309, ; 84: System.IO => 0x514d38cd => 133
	i32 1365406463, ; 85: System.ServiceModel.Internals.dll => 0x516272ff => 121
	i32 1376866003, ; 86: Xamarin.AndroidX.SavedState => 0x52114ed3 => 91
	i32 1395857551, ; 87: Xamarin.AndroidX.Media.dll => 0x5333188f => 83
	i32 1406073936, ; 88: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 63
	i32 1411638395, ; 89: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 34
	i32 1445445088, ; 90: Xamarin.Android.Support.Fragment => 0x5627bde0 => 46
	i32 1457743152, ; 91: System.Runtime.Extensions.dll => 0x56e36530 => 128
	i32 1460219004, ; 92: Xamarin.Forms.Xaml => 0x57092c7c => 105
	i32 1460893475, ; 93: System.IdentityModel.Tokens.Jwt => 0x57137723 => 30
	i32 1462112819, ; 94: System.IO.Compression.dll => 0x57261233 => 117
	i32 1469204771, ; 95: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 52
	i32 1498168481, ; 96: Microsoft.IdentityModel.JsonWebTokens.dll => 0x594c3ca1 => 11
	i32 1516168485, ; 97: Supabase.Gotrue => 0x5a5ee525 => 23
	i32 1543031311, ; 98: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 134
	i32 1571005899, ; 99: zxing.portable => 0x5da3a5cb => 111
	i32 1574652163, ; 100: Xamarin.Android.Support.Core.Utils.dll => 0x5ddb4903 => 45
	i32 1582372066, ; 101: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 67
	i32 1592978981, ; 102: System.Runtime.Serialization.dll => 0x5ef2ee25 => 4
	i32 1622152042, ; 103: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 81
	i32 1624863272, ; 104: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 100
	i32 1636350590, ; 105: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 65
	i32 1639515021, ; 106: System.Net.Http.dll => 0x61b9038d => 3
	i32 1639986890, ; 107: System.Text.RegularExpressions => 0x61c036ca => 134
	i32 1657153582, ; 108: System.Runtime => 0x62c6282e => 35
	i32 1658241508, ; 109: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 94
	i32 1658251792, ; 110: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 106
	i32 1670060433, ; 111: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 62
	i32 1701541528, ; 112: System.Diagnostics.Debug.dll => 0x656b7698 => 129
	i32 1729485958, ; 113: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 58
	i32 1766324549, ; 114: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 93
	i32 1776026572, ; 115: System.Core.dll => 0x69dc03cc => 28
	i32 1788241197, ; 116: Xamarin.AndroidX.Fragment => 0x6a96652d => 70
	i32 1796167890, ; 117: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 8
	i32 1808609942, ; 118: Xamarin.AndroidX.Loader => 0x6bcd3296 => 81
	i32 1813201214, ; 119: Xamarin.Google.Android.Material => 0x6c13413e => 106
	i32 1818569960, ; 120: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 87
	i32 1867746548, ; 121: Xamarin.Essentials.dll => 0x6f538cf4 => 101
	i32 1878053835, ; 122: Xamarin.Forms.Xaml.dll => 0x6ff0d3cb => 105
	i32 1885316902, ; 123: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 55
	i32 1904184254, ; 124: FastAndroidCamera => 0x717f8bbe => 5
	i32 1904755420, ; 125: System.Runtime.InteropServices.WindowsRuntime.dll => 0x718842dc => 2
	i32 1919157823, ; 126: Xamarin.AndroidX.MultiDex.dll => 0x7264063f => 84
	i32 1986222447, ; 127: Microsoft.IdentityModel.Tokens.dll => 0x7663596f => 13
	i32 2011961780, ; 128: System.Buffers.dll => 0x77ec19b4 => 27
	i32 2018526534, ; 129: Supabase.Gotrue.dll => 0x78504546 => 23
	i32 2019465201, ; 130: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 79
	i32 2055257422, ; 131: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 76
	i32 2079903147, ; 132: System.Runtime.dll => 0x7bf8cdab => 35
	i32 2090596640, ; 133: System.Numerics.Vectors => 0x7c9bf920 => 32
	i32 2097448633, ; 134: Xamarin.AndroidX.Legacy.Support.Core.UI => 0x7d0486b9 => 72
	i32 2126786730, ; 135: Xamarin.Forms.Platform.Android => 0x7ec430aa => 103
	i32 2128198166, ; 136: Supabase.Realtime => 0x7ed9ba16 => 25
	i32 2138252982, ; 137: Supabase => 0x7f7326b6 => 21
	i32 2166116741, ; 138: Xamarin.Android.Support.Core.Utils => 0x811c5185 => 45
	i32 2193016926, ; 139: System.ObjectModel.dll => 0x82b6c85e => 130
	i32 2201231467, ; 140: System.Net.Http => 0x8334206b => 3
	i32 2217644978, ; 141: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 96
	i32 2244775296, ; 142: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 82
	i32 2256548716, ; 143: Xamarin.AndroidX.MultiDex => 0x8680336c => 84
	i32 2261435625, ; 144: Xamarin.AndroidX.Legacy.Support.V4.dll => 0x86cac4e9 => 74
	i32 2279755925, ; 145: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 90
	i32 2315684594, ; 146: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 50
	i32 2329204181, ; 147: zxing.portable.dll => 0x8ad4d5d5 => 111
	i32 2330457430, ; 148: Xamarin.Android.Support.Core.UI.dll => 0x8ae7f556 => 44
	i32 2341995103, ; 149: ZXingNetMobile => 0x8b98025f => 112
	i32 2369706906, ; 150: Microsoft.IdentityModel.Logging => 0x8d3edb9a => 12
	i32 2373288475, ; 151: Xamarin.Android.Support.Fragment.dll => 0x8d75821b => 46
	i32 2409053734, ; 152: Xamarin.AndroidX.Preference.dll => 0x8f973e26 => 88
	i32 2431243866, ; 153: ZXing.Net.Mobile.Core.dll => 0x90e9d65a => 108
	i32 2454642406, ; 154: System.Text.Encoding.dll => 0x924edee6 => 132
	i32 2465532216, ; 155: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 61
	i32 2471841756, ; 156: netstandard.dll => 0x93554fdc => 1
	i32 2475788418, ; 157: Java.Interop.dll => 0x93918882 => 7
	i32 2482213323, ; 158: ZXing.Net.Mobile.Forms.Android => 0x93f391cb => 109
	i32 2501346920, ; 159: System.Data.DataSetExtensions => 0x95178668 => 115
	i32 2505896520, ; 160: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 78
	i32 2562349572, ; 161: Microsoft.CSharp => 0x98ba5a04 => 9
	i32 2570120770, ; 162: System.Text.Encodings.Web => 0x9930ee42 => 36
	i32 2581819634, ; 163: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 97
	i32 2620871830, ; 164: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 65
	i32 2624644809, ; 165: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 69
	i32 2633051222, ; 166: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 77
	i32 2640290731, ; 167: Microsoft.IdentityModel.Logging.dll => 0x9d5fa3ab => 12
	i32 2693849962, ; 168: System.IO.dll => 0xa090e36a => 133
	i32 2701096212, ; 169: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 94
	i32 2715334215, ; 170: System.Threading.Tasks.dll => 0xa1d8b647 => 126
	i32 2719963679, ; 171: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 124
	i32 2732626843, ; 172: Xamarin.AndroidX.Activity => 0xa2e0939b => 49
	i32 2735172069, ; 173: System.Threading.Channels => 0xa30769e5 => 38
	i32 2737747696, ; 174: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 52
	i32 2766581644, ; 175: Xamarin.Forms.Core => 0xa4e6af8c => 102
	i32 2778768386, ; 176: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 99
	i32 2810250172, ; 177: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 63
	i32 2819470561, ; 178: System.Xml.dll => 0xa80db4e1 => 39
	i32 2853208004, ; 179: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 99
	i32 2855708567, ; 180: Xamarin.AndroidX.Transition => 0xaa36a797 => 95
	i32 2861816565, ; 181: Rg.Plugins.Popup.dll => 0xaa93daf5 => 19
	i32 2903344695, ; 182: System.ComponentModel.Composition => 0xad0d8637 => 119
	i32 2905242038, ; 183: mscorlib.dll => 0xad2a79b6 => 16
	i32 2916838712, ; 184: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 100
	i32 2919462931, ; 185: System.Numerics.Vectors.dll => 0xae037813 => 32
	i32 2921128767, ; 186: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 51
	i32 2978675010, ; 187: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 68
	i32 3024354802, ; 188: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 73
	i32 3044182254, ; 189: FormsViewGroup => 0xb57288ee => 6
	i32 3057625584, ; 190: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 85
	i32 3075834255, ; 191: System.Threading.Tasks => 0xb755818f => 126
	i32 3084678329, ; 192: Microsoft.IdentityModel.Tokens => 0xb7dc74b9 => 13
	i32 3092211740, ; 193: Xamarin.Android.Support.Media.Compat.dll => 0xb84f681c => 47
	i32 3099081453, ; 194: MimeMapping.dll => 0xb8b83aed => 14
	i32 3111772706, ; 195: System.Runtime.Serialization => 0xb979e222 => 4
	i32 3124832203, ; 196: System.Threading.Tasks.Extensions => 0xba4127cb => 123
	i32 3138360719, ; 197: Supabase.Postgrest.dll => 0xbb0f958f => 24
	i32 3204380047, ; 198: System.Data.dll => 0xbefef58f => 113
	i32 3211777861, ; 199: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 67
	i32 3220365878, ; 200: System.Threading => 0xbff2e236 => 131
	i32 3242291779, ; 201: MSM => 0xc1417243 => 17
	i32 3247949154, ; 202: Mono.Security => 0xc197c562 => 125
	i32 3258312781, ; 203: Xamarin.AndroidX.CardView => 0xc235e84d => 58
	i32 3265893370, ; 204: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 123
	i32 3267021929, ; 205: Xamarin.AndroidX.AsyncLayoutInflater => 0xc2bacc69 => 56
	i32 3299363146, ; 206: System.Text.Encoding => 0xc4a8494a => 132
	i32 3312457198, ; 207: Microsoft.IdentityModel.JsonWebTokens => 0xc57015ee => 11
	i32 3317135071, ; 208: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 66
	i32 3317144872, ; 209: System.Data => 0xc5b79d28 => 113
	i32 3340431453, ; 210: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 55
	i32 3346324047, ; 211: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 86
	i32 3353484488, ; 212: Xamarin.AndroidX.Legacy.Support.Core.UI.dll => 0xc7e21cc8 => 72
	i32 3358260929, ; 213: System.Text.Json => 0xc82afec1 => 37
	i32 3362522851, ; 214: Xamarin.AndroidX.Core => 0xc86c06e3 => 64
	i32 3366347497, ; 215: Java.Interop => 0xc8a662e9 => 7
	i32 3374999561, ; 216: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 90
	i32 3395150330, ; 217: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 34
	i32 3404865022, ; 218: System.ServiceModel.Internals => 0xcaf21dfe => 121
	i32 3429136800, ; 219: System.Xml => 0xcc6479a0 => 39
	i32 3430777524, ; 220: netstandard => 0xcc7d82b4 => 1
	i32 3439690031, ; 221: Xamarin.Android.Support.Annotations.dll => 0xcd05812f => 42
	i32 3441283291, ; 222: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 69
	i32 3476120550, ; 223: Mono.Android => 0xcf3163e6 => 15
	i32 3485117614, ; 224: System.Text.Json.dll => 0xcfbaacae => 37
	i32 3486566296, ; 225: System.Transactions => 0xcfd0c798 => 114
	i32 3493954962, ; 226: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 60
	i32 3501239056, ; 227: Xamarin.AndroidX.AsyncLayoutInflater.dll => 0xd0b0ab10 => 56
	i32 3509114376, ; 228: System.Xml.Linq => 0xd128d608 => 40
	i32 3536029504, ; 229: Xamarin.Forms.Platform.Android.dll => 0xd2c38740 => 103
	i32 3567349600, ; 230: System.ComponentModel.Composition.dll => 0xd4a16f60 => 119
	i32 3607666123, ; 231: Supabase.Postgrest => 0xd7089dcb => 24
	i32 3618140916, ; 232: Xamarin.AndroidX.Preference => 0xd7a872f4 => 88
	i32 3627220390, ; 233: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 89
	i32 3632359727, ; 234: Xamarin.Forms.Platform => 0xd881692f => 104
	i32 3633644679, ; 235: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 51
	i32 3641597786, ; 236: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 76
	i32 3672681054, ; 237: Mono.Android.dll => 0xdae8aa5e => 15
	i32 3676310014, ; 238: System.Web.Services.dll => 0xdb2009fe => 120
	i32 3682565725, ; 239: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 57
	i32 3684561358, ; 240: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 60
	i32 3684933406, ; 241: System.Runtime.InteropServices.WindowsRuntime => 0xdba39f1e => 2
	i32 3689375977, ; 242: System.Drawing.Common => 0xdbe768e9 => 116
	i32 3700591436, ; 243: Microsoft.IdentityModel.Abstractions.dll => 0xdc928b4c => 10
	i32 3718780102, ; 244: Xamarin.AndroidX.Annotation => 0xdda814c6 => 50
	i32 3724971120, ; 245: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 85
	i32 3731644420, ; 246: System.Reactive => 0xde6c6004 => 33
	i32 3758932259, ; 247: Xamarin.AndroidX.Legacy.Support.V4 => 0xe00cc123 => 74
	i32 3779181567, ; 248: MSM.Android.dll => 0xe141bbff => 0
	i32 3786282454, ; 249: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 59
	i32 3822602673, ; 250: Xamarin.AndroidX.Media => 0xe3d849b1 => 83
	i32 3829621856, ; 251: System.Numerics.dll => 0xe4436460 => 31
	i32 3847036339, ; 252: ZXing.Net.Mobile.Forms.Android.dll => 0xe54d1db3 => 109
	i32 3885922214, ; 253: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 95
	i32 3896760992, ; 254: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 64
	i32 3906640997, ; 255: Supabase.Storage.dll => 0xe8da9c65 => 26
	i32 3920810846, ; 256: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 118
	i32 3921031405, ; 257: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 98
	i32 3931092270, ; 258: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 87
	i32 3945713374, ; 259: System.Data.DataSetExtensions.dll => 0xeb2ecede => 115
	i32 3955647286, ; 260: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 53
	i32 3980364293, ; 261: Supabase.Storage => 0xed3f8a05 => 26
	i32 4025784931, ; 262: System.Memory => 0xeff49a63 => 122
	i32 4028990382, ; 263: MSM.dll => 0xf02583ae => 17
	i32 4073602200, ; 264: System.Threading.dll => 0xf2ce3c98 => 131
	i32 4105002889, ; 265: Mono.Security.dll => 0xf4ad5f89 => 125
	i32 4151237749, ; 266: System.Core => 0xf76edc75 => 28
	i32 4182413190, ; 267: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 80
	i32 4186595366, ; 268: ZXing.Net.Mobile.Core => 0xf98a6026 => 108
	i32 4260525087, ; 269: System.Buffers => 0xfdf2741f => 27
	i32 4263231520, ; 270: System.IdentityModel.Tokens.Jwt.dll => 0xfe1bc020 => 30
	i32 4292120959 ; 271: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 80
], align 4
@assembly_image_cache_indices = local_unnamed_addr constant [272 x i32] [
	i32 78, i32 107, i32 18, i32 102, i32 21, i32 92, i32 92, i32 38, ; 0..7
	i32 14, i32 59, i32 43, i32 5, i32 93, i32 57, i32 48, i32 129, ; 8..15
	i32 73, i32 9, i32 135, i32 120, i32 62, i32 77, i32 71, i32 49, ; 16..23
	i32 31, i32 110, i32 75, i32 122, i32 44, i32 61, i32 101, i32 127, ; 24..31
	i32 70, i32 16, i32 29, i32 71, i32 10, i32 82, i32 130, i32 43, ; 32..39
	i32 114, i32 128, i32 8, i32 124, i32 118, i32 33, i32 66, i32 0, ; 40..47
	i32 36, i32 98, i32 54, i32 40, i32 42, i32 41, i32 20, i32 117, ; 48..55
	i32 116, i32 89, i32 135, i32 48, i32 19, i32 22, i32 107, i32 110, ; 56..63
	i32 18, i32 47, i32 75, i32 6, i32 127, i32 91, i32 53, i32 104, ; 64..71
	i32 79, i32 41, i32 29, i32 112, i32 96, i32 86, i32 54, i32 25, ; 72..79
	i32 22, i32 97, i32 68, i32 20, i32 133, i32 121, i32 91, i32 83, ; 80..87
	i32 63, i32 34, i32 46, i32 128, i32 105, i32 30, i32 117, i32 52, ; 88..95
	i32 11, i32 23, i32 134, i32 111, i32 45, i32 67, i32 4, i32 81, ; 96..103
	i32 100, i32 65, i32 3, i32 134, i32 35, i32 94, i32 106, i32 62, ; 104..111
	i32 129, i32 58, i32 93, i32 28, i32 70, i32 8, i32 81, i32 106, ; 112..119
	i32 87, i32 101, i32 105, i32 55, i32 5, i32 2, i32 84, i32 13, ; 120..127
	i32 27, i32 23, i32 79, i32 76, i32 35, i32 32, i32 72, i32 103, ; 128..135
	i32 25, i32 21, i32 45, i32 130, i32 3, i32 96, i32 82, i32 84, ; 136..143
	i32 74, i32 90, i32 50, i32 111, i32 44, i32 112, i32 12, i32 46, ; 144..151
	i32 88, i32 108, i32 132, i32 61, i32 1, i32 7, i32 109, i32 115, ; 152..159
	i32 78, i32 9, i32 36, i32 97, i32 65, i32 69, i32 77, i32 12, ; 160..167
	i32 133, i32 94, i32 126, i32 124, i32 49, i32 38, i32 52, i32 102, ; 168..175
	i32 99, i32 63, i32 39, i32 99, i32 95, i32 19, i32 119, i32 16, ; 176..183
	i32 100, i32 32, i32 51, i32 68, i32 73, i32 6, i32 85, i32 126, ; 184..191
	i32 13, i32 47, i32 14, i32 4, i32 123, i32 24, i32 113, i32 67, ; 192..199
	i32 131, i32 17, i32 125, i32 58, i32 123, i32 56, i32 132, i32 11, ; 200..207
	i32 66, i32 113, i32 55, i32 86, i32 72, i32 37, i32 64, i32 7, ; 208..215
	i32 90, i32 34, i32 121, i32 39, i32 1, i32 42, i32 69, i32 15, ; 216..223
	i32 37, i32 114, i32 60, i32 56, i32 40, i32 103, i32 119, i32 24, ; 224..231
	i32 88, i32 89, i32 104, i32 51, i32 76, i32 15, i32 120, i32 57, ; 232..239
	i32 60, i32 2, i32 116, i32 10, i32 50, i32 85, i32 33, i32 74, ; 240..247
	i32 0, i32 59, i32 83, i32 31, i32 109, i32 95, i32 64, i32 26, ; 248..255
	i32 118, i32 98, i32 87, i32 115, i32 53, i32 26, i32 122, i32 17, ; 256..263
	i32 131, i32 125, i32 28, i32 80, i32 108, i32 27, i32 30, i32 80 ; 272..271
], align 4

@marshal_methods_number_of_classes = local_unnamed_addr constant i32 0, align 4

; marshal_methods_class_cache
@marshal_methods_class_cache = global [0 x %struct.MarshalMethodsManagedClass] [
], align 4; end of 'marshal_methods_class_cache' array


@get_function_pointer = internal unnamed_addr global void (i32, i32, i32, i8**)* null, align 4

; Function attributes: "frame-pointer"="none" "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn writeonly
define void @xamarin_app_init (void (i32, i32, i32, i8**)* %fn) local_unnamed_addr #0
{
	store void (i32, i32, i32, i8**)* %fn, void (i32, i32, i32, i8**)** @get_function_pointer, align 4
	ret void
}

; Names of classes in which marshal methods reside
@mm_class_names = local_unnamed_addr constant [0 x i8*] zeroinitializer, align 4
@__MarshalMethodName_name.0 = internal constant [1 x i8] c"\00", align 1

; mm_method_names
@mm_method_names = local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	; 0
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		i8* getelementptr inbounds ([1 x i8], [1 x i8]* @__MarshalMethodName_name.0, i32 0, i32 0); name
	}
], align 8; end of 'mm_method_names' array


attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable willreturn writeonly "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #1 = { "min-legal-vector-width"="0" mustprogress "no-trapping-math"="true" nounwind sspstrong "stack-protector-buffer-size"="8" uwtable "frame-pointer"="none" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" "stackrealign" }
attributes #2 = { nounwind }

!llvm.module.flags = !{!0, !1, !2}
!llvm.ident = !{!3}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!2 = !{i32 1, !"NumRegisterParameters", i32 0}
!3 = !{!"Xamarin.Android remotes/origin/d17-5 @ 45b0e144f73b2c8747d8b5ec8cbd3b55beca67f0"}
!llvm.linker.options = !{}
