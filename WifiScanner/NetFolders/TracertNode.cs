using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace WifiScanner
{
    /// <summary>
	/// Contains data about a node encountered using Tracert
	/// </summary>
	public class TracertNode
    {
        /// <summary>
        /// Constructs a new object from the IPAddress of the node and the round trip time taken
        /// </summary>
        /// <param name="address"></param>
        /// <param name="roundTripTime"></param>
        internal TracertNode(IPAddress address, long roundTripTime, IPStatus status)
        {
            this._address = address;
            this._roundTripTime = roundTripTime;
            this._status = status;
        }

        private IPAddress _address;

        /// <summary>
        /// The IPAddress of the node
        /// </summary>
        public IPAddress Address
        {
            get { return _address; }
        }

        private long _roundTripTime;

        /// <summary>
        /// The time taken to go to the node and come back to the originating node in milliseconds.
        /// </summary>
        public long RoundTripTime
        {
            get { return _roundTripTime; }
        }

        private IPStatus _status;

        /// <summary>
        /// The IPStatus of request send to the node
        /// </summary>
        public IPStatus Status
        {
            get { return _status; }
        }

    }
}
