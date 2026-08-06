using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2
{
    public class Sdk2UserFaceExDecoder : IDisposable
    {
        private BS2UserFaceExBlob _userBlob;
        private SupremV2DeviceSupportedFeatures _supportedFeatures { get; set; }
        public long UserId { get; }
        public string UserName { get; }
        public BS2User UserMetadata { get; }
        public BS2UserSetting UserSettings { get; }
        public BS2UserSettingEx UserSettingsEx { get; }
        public byte[] Password { get; }
        public byte[] UserProfileImage { get; }
        public List<BS2CSNCard> CardList { get; }
        public List<string> CardNumbers { get; }
        public List<BS2Fingerprint> FingerprintList { get; }
        public List<BS2Face> FaceList { get; }
        public byte[] VisibleLightImage { get; }

        public Sdk2UserFaceExDecoder(BS2UserFaceExBlob userBlob, SupremV2DeviceSupportedFeatures supportedFeatures)
        {
            _userBlob = userBlob;
            _supportedFeatures = supportedFeatures;
            UserMetadata = userBlob.user;
            Password = new byte[BS2Environment.BS2_PIN_HASH_SIZE];
            CardList = new List<BS2CSNCard>();
            CardNumbers = new List<string>();
            FingerprintList = new List<BS2Fingerprint>();
            FaceList = new List<BS2Face>();
            UserSettings = userBlob.setting;
            UserSettingsEx = userBlob.settingEx;
            UserId = SupremaV2Utility.GetUserId(UserMetadata.userID);
            UserName = string.Empty;

            if (supportedFeatures.PhotoSupported && userBlob.user_photo_obj != IntPtr.Zero)
            {
                var userProfile = (BS2UserPhoto)Marshal.PtrToStructure(userBlob.user_photo_obj, typeof(BS2UserPhoto));
                UserProfileImage = userProfile.data.Take((int)userProfile.size).ToArray();
            }
            if (userBlob.name != null)
            {
                UserName = System.Text.Encoding.UTF8.GetString(userBlob.name).TrimEnd(new[] { '\0' });
            }
            if (_supportedFeatures.PinSupported)
            {
                if (userBlob.pin != null)
                {
                    Password = userBlob.pin;
                }
            }
            if (_supportedFeatures.CardSupported)
            {
                if (userBlob.user.numCards > 0)
                {
                    var type = typeof(BS2CSNCard);
                    var structSize = Marshal.SizeOf(type);
                    var currentObjects = userBlob.cardObjs;
                    for (byte i = 0; i < userBlob.user.numCards; ++i)
                    {
                        var currentCard = (BS2CSNCard)Marshal.PtrToStructure(currentObjects, type);
                        CardList.Add(currentCard);
                        currentObjects += structSize;
                        CardNumbers.Add(CardToCardNumber(currentCard));
                    }
                }
            }
            if (_supportedFeatures.FingerSupported && userBlob.user.numFingers > 0)
            {
                var currentFingerObjects = userBlob.fingerObjs;
                var structSize = Marshal.SizeOf(typeof(BS2Fingerprint));
                for (var i = 0; i < userBlob.user.numFingers; i++)
                {
                    var fingerprint =
                        (BS2Fingerprint)Marshal.PtrToStructure(currentFingerObjects, typeof(BS2Fingerprint));
                    fingerprint.index = (byte)(i + 1);
                    FingerprintList.Add(fingerprint);
                    currentFingerObjects = (IntPtr)((long)currentFingerObjects + structSize);
                }
            }

            if (userBlob.user.numFaces > 0)
            {
                if (_supportedFeatures.FaceExScanSupported)
                {
                    if (userBlob.faceExObjs != IntPtr.Zero)
                    {
                        var structType = typeof(BS2FaceExWarped);
                        var structSize = Marshal.SizeOf(structType);
                        var curItem = userBlob.faceExObjs;
                        for (var i = 0; i < userBlob.user.numFaces; i++)
                        {
                            var item = (BS2FaceExWarped)Marshal.PtrToStructure(curItem, structType);
                            VisibleLightImage = item.imageData;
                            curItem += structSize;
                        }
                    }
                }
                else if (_supportedFeatures.FaceScanSupported)
                {
                    var currentFaceObjects = userBlob.faceObjs;
                    var structSize = Marshal.SizeOf(typeof(BS2Face));
                    for (var i = 0; i < userBlob.user.numFaces; i++)
                    {
                        var face = (BS2Face)Marshal.PtrToStructure(currentFaceObjects, typeof(BS2Face));
                        FaceList.Add(face);
                        currentFaceObjects = (IntPtr)((long)currentFaceObjects + structSize);
                    }
                }
                
            }

            

            
        }


        public static string CardToCardNumber(BS2CSNCard card)
        {
            return BitConverter.ToInt64(card.data.Reverse().ToArray(), 0).ToString();
        }

        public static string CardToCardNumber(BS2Card card)
        {
            var cardConverted = SupremaV2Utility.ConvertTo<BS2CSNCard>(card.cardUnion);
            return CardToCardNumber(cardConverted);
        }

        public static BS2CSNCard CardNumberToCard(string cardNumber, byte type, byte size)
        {
            var cardNumberLong = cardNumber.ToInt64();
            var bytesOfCard = new byte[BS2Environment.BS2_CARD_DATA_SIZE];
            var bytesOfLong = BitConverter.GetBytes(cardNumberLong);
            Buffer.BlockCopy(BitConverter.GetBytes(cardNumberLong), 0, bytesOfCard, 0, bytesOfLong.Length);
            var result = SupremaV2Utility.AllocateStructure<BS2CSNCard>();
            result.type = type;
            result.size = size;
            result.data = bytesOfCard.Reverse().ToArray();
            return result;
        }




        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Free any other managed objects here.
            }

            if (_userBlob.cardObjs != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseObject(_userBlob.cardObjs);
                _userBlob.cardObjs = IntPtr.Zero;
            }
            if (_userBlob.fingerObjs != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseObject(_userBlob.fingerObjs);
                _userBlob.fingerObjs = IntPtr.Zero;
            }
            if (_userBlob.faceObjs != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseObject(_userBlob.faceObjs);
                _userBlob.faceObjs = IntPtr.Zero;
            }
            if (_userBlob.faceExObjs != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseObject(_userBlob.faceExObjs);
                _userBlob.faceExObjs = IntPtr.Zero;
            }
            if (_userBlob.user_photo_obj != IntPtr.Zero)
            {
                ApiV2.BS2_ReleaseObject(_userBlob.user_photo_obj);
                _userBlob.user_photo_obj = IntPtr.Zero;
            }
           
            _disposed = true;
        }
       
        ~Sdk2UserFaceExDecoder()
        {
            Dispose(false);
        }

        #endregion

    }
}