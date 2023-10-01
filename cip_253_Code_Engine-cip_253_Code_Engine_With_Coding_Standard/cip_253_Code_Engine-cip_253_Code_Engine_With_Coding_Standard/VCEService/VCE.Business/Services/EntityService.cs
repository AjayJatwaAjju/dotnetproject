using System.Data;
using VCE.DAL;

namespace VCE.Business
{
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Summary description for Service.
    /// </summary>
    /// <remarks>
    /// Created On:- 20/09/2023
    /// Created By:- VCE
    /// Purpose:- This class will provide service for Provider.
    /// </remarks>
    ///---------------------------------------------------------------------------
    public class EntityService
    {
        #region Methods
        /// <summary>
        ///This will return all Table records based on applied condition.
        /// </summary>
        /// <param name="whereCondition">Passing condition for filtering base_export records</param>
        /// <param name="orderBy">Passing OrderBy</param>
        /// <returns>returning base_exports which is generated.</returns>
        private readonly EntityProvider _entityProvider;

        public EntityService(EntityProvider entityProvider)
        {
            _entityProvider = entityProvider;
        }
        /// <summary>
        /// Inserting base_exports information.
        /// </summary>
        /// <remarks>
        /// <param name="Insertbase_exportInfo">Passing all base_exports info."</param>
        /// <returns>returning base_export_id which is generated.</returns>
        /// </remarks>
        public int InsertUser(DataTable userInfo)
        {
            return _entityProvider.Insert(userInfo);
        }
        /// <summary>
        /// Updating  base_exports information.
        /// </summary>
        /// <remarks>
        /// <param name="base_exportsInfo">Passing all base_exports info.</param>
        /// <returns>returning updated record status. </returns>
        /// </remarks>
        public int UpdateUser(DataTable userInfo)
        {
            return _entityProvider.Update(userInfo);
        }
        /// <summary>
        /// This will delete base_exports table record based on UserId.
        /// </summary>
        /// <remarks/>
        /// <param name="base_export_id">Passing deleteId.</param>
        /// <returns>returning delete status.</returns>
        /// <remarks/>
        public bool DeleteUser(string userID)
        {
            return _entityProvider.Delete(userID);
        }

        /// <summary>
        /// Inserting base_exports information.
        /// </summary>
        /// <remarks>
        /// <param name="Insertbase_exportInfo">Passing all base_exports info."</param>
        /// <returns>returning base_export_id which is generated.</returns>
        /// </remarks>
        public DataTable GetUserInfo(string whereCondition, string orderBy, string pageStart, string pageSize)
        {
            return _entityProvider.GetUserInfo(whereCondition, orderBy, pageStart, pageSize);
        }
        /// <summary>
        /// Get base_export info by Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>returning Id which is generated.</returns>
        public DataTable GetUserInfoById(string userId)
        {
            return _entityProvider.GetUserInfoById(userId);
        } 
        #endregion
    }
}