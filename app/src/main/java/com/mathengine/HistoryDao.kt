package com.mathengine.pro

import androidx.room.*
import kotlinx.coroutines.flow.Flow

@Dao
interface HistoryDao {
    @Query("SELECT * FROM history ORDER BY timestamp DESC")
    fun getAll(): List<MainActivity.HistoryItem>
    
    @Insert
    suspend fun insert(item: MainActivity.HistoryItem)
    
    @Delete
    suspend fun delete(item: MainActivity.HistoryItem)
    
    @Query("DELETE FROM history")
    suspend fun deleteAll()
}

@Database(entities = [MainActivity.HistoryItem::class], version = 1)
abstract class MathDatabase : RoomDatabase() {
    abstract fun historyDao(): HistoryDao
    
    companion object {
        @Volatile
        private var INSTANCE: MathDatabase? = null
        
        fun getInstance(context: Context): MathDatabase {
            return INSTANCE ?: synchronized(this) {
                val instance = Room.databaseBuilder(
                    context.applicationContext,
                    MathDatabase::class.java,
                    "math_database"
                ).build()
                INSTANCE = instance
                instance
            }
        }
    }
}

@Entity(tableName = "history")
data class HistoryItem(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val input: String,
    val output: String,
    val timestamp: Long
)