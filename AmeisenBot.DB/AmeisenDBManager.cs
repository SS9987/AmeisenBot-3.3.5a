using AmeisenBotData;
using AmeisenBotLogger;
using AmeisenBotMapping.objects;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Objects;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AmeisenBotDB
{
    public class AmeisenDBManager
    {
        public const string TABLE_NAME_NODES = "ameisenbot_map_nodes";
        public const string TABLE_NAME_REMEMBERED_UNITS = "ameisenbot_remembered_units";
        public string DBName = "ameisenbot";
        private string MysqlConnectionString { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }

        public AmeisenDBManager(AmeisenDataHolder ameisenDataHolder)
        {
            AmeisenDataHolder = ameisenDataHolder;
            ameisenDataHolder.IsConnectedToDB = false;
        }

        public List<RememberedUnit> GetRememberedUnits(UnitTrait unitTrait)
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                List<RememberedUnit> unitsToReturn = new List<RememberedUnit>();
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                sqlConnection.Open();

                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($"SELECT * FROM {TABLE_NAME_REMEMBERED_UNITS} ");

                List<dynamic> rawUnits = sqlConnection.Query(sqlQuery.ToString()).AsList();

                foreach (dynamic rawUnit in rawUnits)
                {
                    try
                    {
                        RememberedUnit rememberedUnit = new RememberedUnit
                        {
                            Name = rawUnit.name,
                            ZoneID = rawUnit.zone_id,
                            MapID = rawUnit.map_id,
                            Position = new Vector3(rawUnit.x, rawUnit.y, rawUnit.z),
                            UnitTraitsString = rawUnit.traits
                        };

                        rememberedUnit.UnitTraits = JsonConvert.DeserializeObject<List<UnitTrait>>(rememberedUnit.UnitTraitsString);
                        unitsToReturn.Add(rememberedUnit);
                    }
                    catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, "Error Parsing RememberedUnit...", this); }
                    finally { sqlConnection.Close(); }
                }

                return unitsToReturn;
            }
            return new List<RememberedUnit>();
        }

        /// <summary>
        /// Connect to a MySQL database
        /// </summary>
        /// <param name="mysqlConnectionString">mysql connection string</param>
        /// <returns>true if connected, false if not</returns>
        public bool ConnectToMySQL(string mysqlConnectionString)
        {
            if (!AmeisenDataHolder.IsConnectedToDB)
            {
                MySqlConnection sqlConnection = new MySqlConnection(mysqlConnectionString);
                MysqlConnectionString = mysqlConnectionString;
                try
                {
                    sqlConnection.Open();
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Connected to MySQL DB", this);
                    AmeisenDataHolder.IsConnectedToDB = true;
                    InitDB();
                    sqlConnection.Close();
                }
                catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Connection to MySQL failed... connectionString: {mysqlConnectionString}", this); }
            }
            return AmeisenDataHolder.IsConnectedToDB;
        }

        /// <summary>
        /// Get all saved nodes by the zone & map id
        /// </summary>
        /// <param name="zoneID">zone id to get the nodes from</param>
        /// <param name="mapID">map id to get the nodes from</param>
        /// <returns>list containing all the MapNodes</returns>
        public List<MapNode> GetNodes(int zoneID, int mapID, int maxX = 0, int minX = 0, int maxY = 0, int minY = 0)
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                sqlConnection.Open();

                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($"SELECT * FROM {TABLE_NAME_NODES} ");
                sqlQuery.Append($"WHERE zone_id = {zoneID} AND ");
                sqlQuery.Append($"map_id = {mapID} ");

                if (maxX >= minX) { sqlQuery.Append($"AND (x BETWEEN {minX} AND {maxX})"); }
                else { sqlQuery.Append($"AND (x BETWEEN {maxX} AND {minX})"); }

                if (maxX >= minX) { sqlQuery.Append($"AND (y BETWEEN {minY} AND {maxY})"); }
                else { sqlQuery.Append($"AND (y BETWEEN {maxY} AND {minY})"); }

                sqlQuery.Append(";");

                List<MapNode> nodeList = sqlConnection.Query<MapNode>(sqlQuery.ToString()).AsList();
                sqlConnection.Close();
                return nodeList;
            }
            return new List<MapNode>();
        }

        /// <summary>
        /// Initialise the database with Tables
        /// </summary>
        public void InitDB()
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                StringBuilder dbInit = new StringBuilder();

                AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Initializing MySQL DB...", this);

                dbInit.Append($"CREATE DATABASE IF NOT EXISTS `{sqlConnection.Database}` /*!40100 DEFAULT CHARACTER SET utf8 */;");
                dbInit.Append($"USE `{sqlConnection.Database}`;");
                dbInit.Append($"CREATE TABLE IF NOT EXISTS `{TABLE_NAME_NODES}` (");
                dbInit.Append("`id` int(11) NOT NULL AUTO_INCREMENT, ");
                dbInit.Append("`x` int(11) DEFAULT NULL, ");
                dbInit.Append("`y` int(11) DEFAULT NULL, ");
                dbInit.Append("`z` int(11) DEFAULT NULL, ");
                dbInit.Append("`zone_id` int(11) DEFAULT NULL,");
                dbInit.Append("`map_id` int(11) DEFAULT NULL,");
                dbInit.Append("PRIMARY KEY(`id`), ");
                dbInit.Append("UNIQUE KEY `coordinates` (`x`,`y`,`z`,`zone_id`,`map_id`) ");
                dbInit.Append(") ENGINE = InnoDB DEFAULT CHARSET = utf8;");

                dbInit.Append($"CREATE DATABASE IF NOT EXISTS `{sqlConnection.Database}` /*!40100 DEFAULT CHARACTER SET utf8 */;");
                dbInit.Append($"USE `{sqlConnection.Database}`;");
                dbInit.Append($"CREATE TABLE IF NOT EXISTS `{TABLE_NAME_REMEMBERED_UNITS}` (");
                dbInit.Append("`id` int(11) NOT NULL AUTO_INCREMENT, ");
                dbInit.Append("`name` text, ");
                dbInit.Append("`x` double DEFAULT '0', ");
                dbInit.Append("`y` double DEFAULT '0', ");
                dbInit.Append("`z` double DEFAULT '0', ");
                dbInit.Append("`zone_id` int(11) DEFAULT '0', ");
                dbInit.Append("`map_id` int(11) DEFAULT '0', ");
                dbInit.Append("`traits` json DEFAULT NULL, ");
                dbInit.Append("PRIMARY KEY(`id`), ");
                dbInit.Append("UNIQUE KEY `guid` (`x`,`y`,`z`,`zone_id`,`map_id`) ");
                dbInit.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8;");

                sqlConnection.Execute(dbInit.ToString());
                sqlConnection.Close();
                AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Initialized MySQL DB", this);
            }
        }

        /// <summary>
        /// Add a MapNode to the database, duplicate nodes will be ignored
        /// </summary>
        /// <param name="mapNode">Node to add</param>
        /// <returns>affected SQL rows</returns>
        public int UpdateOrAddNode(MapNode mapNode)
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                sqlConnection.Open();
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append("INSERT INTO ");
                sqlQuery.Append(TABLE_NAME_NODES + " (x, y, z, zone_id, map_id) ");
                sqlQuery.Append($"VALUES({mapNode.X}, {mapNode.Y}, {mapNode.Z}, {mapNode.ZoneID}, {mapNode.MapID}) ");
                sqlQuery.Append($"ON DUPLICATE KEY UPDATE zone_id = {mapNode.ZoneID}, map_id = {mapNode.MapID};");

                // very rarely getting weird deadlock exception
                int affectedRows = 0;

                try { affectedRows = sqlConnection.Execute(sqlQuery.ToString()); }
                catch { /* duplicate rows throw an error lel */ }

                sqlConnection.Close();
                return affectedRows;
            }
            return -1;
        }

        public void RememberUnit(RememberedUnit rememberedUnit)
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                sqlConnection.Open();
                rememberedUnit.UnitTraitsString = JsonConvert.SerializeObject(rememberedUnit.UnitTraits);

                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($"INSERT INTO {TABLE_NAME_REMEMBERED_UNITS} (name, x, y, z, zone_id, map_id, traits) ");
                sqlQuery.Append($"VALUES(\"{rememberedUnit.Name}\",");
                sqlQuery.Append($"{(int)rememberedUnit.Position.X},");
                sqlQuery.Append($"{(int)rememberedUnit.Position.Y},");
                sqlQuery.Append($"{(int)rememberedUnit.Position.Z},");
                sqlQuery.Append($"{rememberedUnit.ZoneID},");
                sqlQuery.Append($"{rememberedUnit.MapID},");
                sqlQuery.Append($"\"{rememberedUnit.UnitTraitsString}\") ");
                sqlQuery.Append("ON DUPLICATE KEY UPDATE;");

                try { sqlConnection.Execute(sqlQuery.ToString()); }
                catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Error adding RememberedUnit: {JsonConvert.SerializeObject(rememberedUnit)}", this); }
                finally { sqlConnection.Close(); }
            }
        }

        public RememberedUnit CheckForRememberedUnit(string unitname, int zoneID, int mapID)
        {
            if (AmeisenDataHolder.IsConnectedToDB)
            {
                RememberedUnit unitToReturn = null;
                MySqlConnection sqlConnection = new MySqlConnection(MysqlConnectionString);
                sqlConnection.Open();

                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.Append($"SELECT * FROM {TABLE_NAME_REMEMBERED_UNITS} ");
                sqlQuery.Append($"WHERE zone_id = {zoneID} AND ");
                sqlQuery.Append($"map_id = {mapID} AND ");
                sqlQuery.Append($"name = \"{unitname}\";");

                try
                {
                    dynamic rawUnit = sqlConnection.Query(sqlQuery.ToString()).FirstOrDefault();
                    RememberedUnit rememberedUnit = new RememberedUnit
                    {
                        Name = rawUnit.name,
                        ZoneID = rawUnit.zone_id,
                        MapID = rawUnit.map_id,
                        Position = new Vector3(rawUnit.x, rawUnit.y, rawUnit.z),
                        UnitTraitsString = rawUnit.traits
                    };

                    rememberedUnit.UnitTraits = JsonConvert.DeserializeObject<List<UnitTrait>>(rememberedUnit.UnitTraitsString);
                    unitToReturn = rememberedUnit;
                }
                catch
                {
                    AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Error checking for RememberedUnit: unitName: {unitname}, zoneId: {zoneID}, mapId: {mapID}", this);
                    unitToReturn = null;
                }
                finally { sqlConnection.Close(); }
                return unitToReturn;
            }
            return null;
        }
    }
}