namespace GuardianCommunication.Hardware.Pw.PwConcepts
{
	public enum PwErrorEnum : short
	{
		
		LINK_OK = 0,
		
		ERR_No_RecordToReadout = 162,
		
		ERR_LINK_TO = 1001,
		
		ERR_LINK_ERR = 1002,
		
		ERR_BAUD_SET = 1003,
		
		ERR_BAD_MODEM_ID = 1004,
		
		ERR_BONES_CREATE = 1005,
		
		ERR_BONES_OPEN = 1006,
		
		ERR_BONES_WRITE = 1007,
		
		ERR_NOT_EQUAL_LEN = 1008,
		
		ERR_NO_RECORDS = 1009,
		
		ERR_TEXT_CREATE = 1010,
		
		ERR_FILE_CREATE = 2000,
		
		ERR_BAD_SUB = 2001,
		
		ERR_BAD_PTRS = 2002,
		
		ERR_FILE_OPEN = 2003,
		
		ERR_TEXT_FILE = 2004,
		
		ERR_MORE_RECORDS = 2005,
		
		ERR_SEND_DATA = 2006,
		
		ERR_SOCKET_GET = 2007,
		
		ERR_SOCKET_SEND = 2008,
		
		ERR_BAD_DEVICE = 2009, //cmd is not related to this device.
		
		ERR_OLD_VERSION = 2010,  //device is old version.
		
		ERR_GET_DAYS_FAILED = 2011,  //in new recovery.
		
		ERR_BAD_DATA_LEN = 2012,  //
		
		ERR_SAVE_IN_STRUCT = 2013,  //saving records in struct failed(len maybe < records)
		
		ERR_BAD_COOL_LEN = 2014,  //bad cooldisk file len
		
		ERR_EMPTY_COOLDISK_FILE = 2015,
		
		ERR_BAD_COOLDISK_FILE = 2016,
		
		ERR_MEDIA_OPEN_ERROR = 2017,
		
		ERR_MEDIA_CLOSE_ERROR = 2018,
		
		ERR_BAD_SEQ_VALUE = 2019,
		
		ERR_BAD_FILE_LEN = 2020,
		
		ERR_REGISTERED = 2021,  //already
		
		ERR_BAD_TMPL_LEN = 2022,
		
		ERR_INVALID_FILE = 2023,
		
		ERR_NOT_REGISTERED = 2024,
		
		ERR_BAD_REPEAT_SECS = 2025,
		
		ERR_NO_ITEMS_TO_SEND = 2026,
		
		ERR_BAD_GRP_CODE = 2027,
		
		errInvalidId = 2028,//22,
		
		errInvalidIndex = 2029,//       = 23,
		
		errReadingData = 2030,//24,
		
		ERR_MV_TIMEOUT = 2031,
		
		ERR_FILE_CHKSUM = 2032,
		
		ERR_FILE_NAME_LEN = 2033,  //bad file name for fp
		
		ERR_FILE_FNO_LEN = 2034,  //bad file name finger no len.
		
		ERR_FILE_CHK_LEN = 2035,  //bad file name chk len.
		
		ERR_FILE_BAD_AMOUNT = 2036,  //fno and chk in fp name is not ok.
		
		ERR_NOT_EQUAL_CHK = 2037,   //fp name chk not equal with fp header.
		
		ERR_NOT_EQUAL_FNO = 2038,   //also for finger no.

		
		ERR_POOR_Security = 2039,
		
		ERR_Authentication_Needed = 2040,
		
		ERR_Authentication_Pass_Needed = 2041,
		
		ERR_Change_Default_Key = 2042,
		
		ERR_Authentication_Max_Tries = 2043,
		
		ERR_Authentication_Failed = 2044,
		
		ERR_Invalid_Key_Amount = 2045
	}

}
