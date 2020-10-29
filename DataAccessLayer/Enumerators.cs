/*=============================================================================================================
 * <Organisation> Ezeelo Consumer Services Pvt. Ltd. </Organisation>
 * 
 * <Copyrights> 
 *  Copyrights to NSP Futuretech. Pvt. Ltd. 
 *  All contents are not subject to change before prior permission of the author or copyright owner</Copyrights>
 *  
 * <Author> Prashant N. Bhoyar </Author>
 * 
 * <CreationDate> MAY 21, 2015 5.30pm </CreationDate>
 * 
 * <Version>1.0.0</Version>
 * ============================================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    /// <summary>
    /// Common Enumerators which are used within project or this library
    /// </summary>
    public class Enumerators
    {
        /// <summary>
        /// This Enumerator defines the constants for operations to be performed with the database
        /// </summary>
        public enum DB_OPERATIONS
        {
            /// <summary>
            /// This Is Defaut for NO Operation to be performed
            /// </summary>
            NONE,

            /// <summary>
            /// The Enumretor to tell about SELECT operation to be performed
            /// </summary>
            SELECT,

            /// <summary>
            /// The Enumretor to tell about INSERT operation to be performed
            /// </summary>
            INSERT,

            /// <summary>
            /// The Enumretor to tell about UPDATE operation to be performed
            /// </summary>
            UPDATE,

            /// <summary>
            /// The Enumretor to tell about DELETE operation to be performed
            /// </summary>
            DELETE,

            /// <summary>
            /// The Enumretor to tell about SEARCH operation to be performed
            /// </summary>
            SEARCH,

            /// <summary>
            /// The Enumretor to tell about any other operation to be performed
            /// </summary>
            OTHER
        }
    }
}
