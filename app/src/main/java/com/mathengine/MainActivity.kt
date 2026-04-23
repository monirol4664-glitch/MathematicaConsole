package com.mathengine.pro

import android.os.Bundle
import android.text.method.ScrollingMovementMethod
import android.view.KeyEvent
import android.view.View
import android.view.inputmethod.EditorInfo
import android.widget.*
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.google.android.material.button.MaterialButton
import com.google.android.material.card.MaterialCardView
import com.google.android.material.textfield.TextInputEditText
import com.google.android.material.textfield.TextInputLayout
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class MainActivity : AppCompatActivity() {
    
    private lateinit var inputEditText: TextInputEditText
    private lateinit var outputTextView: TextView
    private lateinit var historyRecyclerView: RecyclerView
    private lateinit var mathEngine: MathEngine
    private lateinit var historyAdapter: HistoryAdapter
    private val historyList = mutableListOf<HistoryItem>()
    
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)
        supportActionBar?.title = "MathEngine Pro"
        
        mathEngine = MathEngine(this)
        historyAdapter = HistoryAdapter(historyList) { item ->
            inputEditText.setText(item.input)
            evaluateExpression(item.input)
        }
        
        setupViews()
        setupClickListeners()
        
        // Load history
        lifecycleScope.launch {
            historyList.addAll(mathEngine.getHistory())
            historyAdapter.notifyDataSetChanged()
        }
    }
    
    private fun setupViews() {
        inputEditText = findViewById(R.id.inputEditText)
        outputTextView = findViewById(R.id.outputTextView)
        outputTextView.movementMethod = ScrollingMovementMethod()
        
        historyRecyclerView = findViewById(R.id.historyRecyclerView)
        historyRecyclerView.layoutManager = LinearLayoutManager(this)
        historyRecyclerView.adapter = historyAdapter
        
        inputEditText.setOnEditorActionListener { _, actionId, _ ->
            if (actionId == EditorInfo.IME_ACTION_GO) {
                evaluateExpression(inputEditText.text.toString())
                true
            } else false
        }
    }
    
    private fun setupClickListeners() {
        findViewById<MaterialButton>(R.id.evaluateButton).setOnClickListener {
            evaluateExpression(inputEditText.text.toString())
        }
        
        findViewById<MaterialButton>(R.id.clearButton).setOnClickListener {
            inputEditText.text?.clear()
            outputTextView.text = "Ready"
        }
        
        // Math function buttons
        val functionButtons = mapOf(
            R.id.btnIntegrate to "Integrate(sin(x)^2, x)",
            R.id.btnDerivative to "D(cos(x), x)",
            R.id.btnSolve to "Solve(x^3-1==0, x)",
            R.id.btnMatrix to "{{1,2},{3,4}}.Inverse()",
            R.id.btnStats to "Mean({1,2,3,4,5})",
            R.id.btnPlot to "Plot(x^2, x, -5, 5)"
        )
        
        functionButtons.forEach { id, command ->
            findViewById<MaterialButton>(id).setOnClickListener {
                inputEditText.setText(command)
                evaluateExpression(command)
            }
        }
    }
    
    private fun evaluateExpression(input: String) {
        if (input.isBlank()) return
        
        outputTextView.text = "Calculating..."
        
        lifecycleScope.launch(Dispatchers.IO) {
            try {
                val result = mathEngine.evaluate(input)
                
                withContext(Dispatchers.Main) {
                    outputTextView.text = result
                    
                    // Save to history
                    val historyItem = HistoryItem(
                        input = input,
                        output = result,
                        timestamp = System.currentTimeMillis()
                    )
                    historyList.add(0, historyItem)
                    mathEngine.saveHistory(historyItem)
                    historyAdapter.notifyItemInserted(0)
                    historyRecyclerView.scrollToPosition(0)
                    
                    inputEditText.text?.clear()
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    outputTextView.text = "Error: ${e.message}"
                }
            }
        }
    }
    
    data class HistoryItem(
        val input: String,
        val output: String,
        val timestamp: Long
    )
}