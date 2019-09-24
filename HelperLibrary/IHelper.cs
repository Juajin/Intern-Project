using System;
using System.Collections.Generic;
namespace HelperLibrary
{
    public interface IHelper
    {
        /// <summary>
        /// This function allows to save item into database.
        /// </summary>
        /// <typeparam name="T">T must be class and must include Name and ReleaseDate properties. </typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);
        /// <summary>
        /// This function allows to save item into database.
        /// </summary>
        /// <typeparam name="T">T must be class and must include Name and ReleaseDate properties. </typeparam>
        /// <param name="items"></param>
        void SaveList<T>(IEnumerable<T> items);
        /// <summary>
        /// This function allows to get item page by page
        /// </summary>
        /// <typeparam name="T">T must be class and must include Name and ReleaseDate properties. </typeparam>
        /// <param name="TableName">Database table name.</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="sort"><para>true for ->Ascending
        /// </para> <para>false for ->Descending</para></param>
        /// <returns></returns>
        List<T> GetPage<T>(string TableName, int pageSize, bool sort);
        /// <summary>
        /// This function allows to get item with defined attribute name and value.
        /// </summary>
        /// <typeparam name="T">T must be class and must include Name and ReleaseDate properties. </typeparam>
        /// <param name="filterType">Database attribute name.(Global secondary index name must be same with attribute name that will be queried.)</param>
        /// <param name="filterValue">Database attribute value.</param>
        /// <returns></returns>
        List<T> GetItem<T>(string filterType, string filterValue);
        /// <summary>
        /// This function allows to get item from the given dates.
        /// </summary>
        /// <typeparam name="T">T must be class and must include Name and ReleaseDate properties. </typeparam>
        /// <param name="date1">The first date</param>
        /// <param name="date2">The second date</param>
        /// <returns></returns>
        List<T> GetItemByDate<T>(DateTime date1,DateTime date2);
        /// <summary>
        /// <para>This function allows to delete item from database.</para>
        /// <para>NOTE:This is not actual delete function. This funcion changes record-instance availabilty false from true.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void DeleteItem<T>(T item);
    }
}
