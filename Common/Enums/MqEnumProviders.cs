namespace Common.Enums
{
    public enum ClientProvidedNameEnum
    {
        None = 0,
        Upwork = 1,
    }


    public enum ExchangeNameEnum
    {
        None = 0,
        Upwork = 1
    }


    public enum QueueNameOrRouteKeyEnums
    {
        None = 0,
        EmailMessages=1,
        SmsMessages =2,

        GeneralEmailKey = 3,
        GeneralSMSKey = 4,

        JobUserDetailsQueue = 5,
        JobUserDetailsKey = 6,

        PaymentUserDetailsQueue = 7,
        PaymentUserDetailsKey = 8,

        TalentUserDetailsQueue = 9,
        TalentUserDetailsKey = 10,

        InAppNotificationQueue = 11,
        InAppNotificationKey = 12,


    }

   
}
