namespace Ncqrs.CommandService
{
    using System;
    using System.ServiceModel;

    /// <summary>
    /// WCF client channel helper to ease proper disposal of the channels.
    /// </summary>
    public static class ChannelHelper
    {
        ///<summary>
        /// Properly handles the disposal of the communication object.
        ///</summary>
        public static void ProperClose(ICommunicationObject communicationObject)
        {
            if (communicationObject == null)
                return;

            try
            {
                if (communicationObject.State == CommunicationState.Opened)
                {
                    communicationObject.Close();
                }
            }
            finally
            {
                if (communicationObject.State != CommunicationState.Closed)
                {
                    communicationObject.Abort();
                }
            }
        }

        ///<summary>
        /// Encapsulates a call on the WCF channel and properly disposes it afterwards.
        ///</summary>
        ///<param name="channel">Channel do to the call on.</param>
        ///<param name="action">Action to perform.</param>
        ///<typeparam name="TChannel">WCF channel type.</typeparam>
        public static void Use<TChannel>(TChannel channel, Action<TChannel> action)
            where TChannel : ICommunicationObject
        {
            try
            {
                action(channel);
            }
            finally
            {
                ProperClose(channel);
            }
        }
    }
}