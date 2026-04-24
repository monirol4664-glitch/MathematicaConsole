let pyodide = null;
let editor = null;
let originalCode = `# Welcome to Offline Python Compiler!
# Try these examples:

import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

# Basic calculations
print("Hello from Python!")
print(f"2 + 2 = {2 + 2}")

# NumPy example
arr = np.array([1, 2, 3, 4, 5])
print(f"\\nNumPy array: {arr}")
print(f"Mean: {np.mean(arr)}")
print(f"Sum: {np.sum(arr)}")

# For matplotlib plots, use this:
# plt.plot([1, 2, 3, 4], [1, 4, 2, 3])
# plt.show()
`;

// Initialize Pyodide
async function initializePyodide() {
    const statusDiv = document.getElementById('status');
    statusDiv.textContent = 'Loading Python environment...';
    statusDiv.style.background = 'rgba(255,255,255,0.3)';
    
    try {
        pyodide = await loadPyodide({
            indexURL: "https://cdn.jsdelivr.net/pyodide/v0.25.0/full/",
            fullStdLib: true
        });
        
        statusDiv.textContent = 'Loading packages (NumPy, Matplotlib, Pandas, SciPy)...';
        
        // Load common scientific libraries
        await pyodide.loadPackage(['numpy', 'matplotlib', 'pandas', 'scipy']);
        
        // Setup matplotlib for inline display
        pyodide.runPython(`
            import matplotlib
            matplotlib.use('module://matplotlib.backends.backend_agg')
            import matplotlib.pyplot as plt
            import numpy as np
            import pandas as pd
            from scipy import stats
            
            # Custom print function to handle matplotlib figures
            import io
            import base64
            from matplotlib.figure import Figure
            
            original_show = plt.show
            def show_with_base64(*args, **kwargs):
                fig = plt.gcf()
                buf = io.BytesIO()
                fig.savefig(buf, format='png', bbox_inches='tight')
                buf.seek(0)
                img_data = base64.b64encode(buf.getvalue()).decode()
                print(f'<img src="data:image/png;base64,{img_data}" style="max-width:100%;">')
                plt.clf()
            
            plt.show = show_with_base64
        `);
        
        statusDiv.textContent = '✅ Ready! Start coding';
        statusDiv.style.background = 'rgba(81, 207, 102, 0.3)';
        
        setTimeout(() => {
            statusDiv.style.background = 'rgba(255,255,255,0.2)';
        }, 3000);
        
    } catch (error) {
        statusDiv.textContent = '❌ Failed to load: ' + error.message;
        console.error('Pyodide initialization failed:', error);
    }
}

// Initialize CodeMirror with Python mode and autocomplete
function initializeEditor() {
    editor = CodeMirror.fromTextArea(document.getElementById('codeEditor'), {
        lineNumbers: true,
        mode: 'python',
        theme: 'monokai',
        indentUnit: 4,
        lineWrapping: true,
        autoCloseBrackets: true,
        matchBrackets: true,
        extraKeys: {
            'Ctrl-Space': 'autocomplete',
            'Ctrl-Enter': runCode,
            'Tab': function(cm) {
                cm.replaceSelection('    ', 'end');
            }
        },
        hintOptions: {
            completeSingle: false,
            hint: CodeMirror.hint.python
        }
    });
    
    editor.setValue(originalCode);
    
    // Enable autocomplete on typing
    editor.on('inputRead', function(cm, change) {
        if (change.text[0].match(/[a-zA-Z0-9_\.]/)) {
            cm.showHint({completeSingle: false});
        }
    });
}

// Run Python code
async function runCode() {
    if (!pyodide) {
        showOutput('Error: Python environment not ready. Please wait for initialization.', true);
        return;
    }
    
    const code = editor.getValue();
    const outputDiv = document.getElementById('output');
    outputDiv.innerHTML = '';
    
    // Override stdout capture
    let output = '';
    let error = null;
    
    try {
        // Redirect stdout
        pyodide.runPython(`
            import sys
            from io import StringIO
            sys.stdout = StringIO()
        `);
        
        // Execute the code
        await pyodide.runPythonAsync(code);
        
        // Get captured output
        const capturedOutput = pyodide.runPython('sys.stdout.getvalue()');
        if (capturedOutput) {
            output = capturedOutput;
        }
        
        if (!output) {
            output = '✓ Code executed successfully (no output)';
        }
        
        showOutput(output, false);
        
    } catch (err) {
        showOutput(`Error:\n${err.message || err}`, true);
    } finally {
        // Reset stdout
        pyodide.runPython(`
            sys.stdout = sys.__stdout__
        `);
    }
}

function showOutput(text, isError) {
    const outputDiv = document.getElementById('output');
    const pre = document.createElement('pre');
    pre.textContent = text;
    pre.style.color = isError ? '#ff6b6b' : '#d4d4d4';
    pre.style.whiteSpace = 'pre-wrap';
    pre.style.wordWrap = 'break-word';
    outputDiv.appendChild(pre);
    
    // Handle any HTML content (like images from matplotlib)
    if (text.includes('<img')) {
        outputDiv.innerHTML = text;
    }
}

function clearOutput() {
    document.getElementById('output').innerHTML = '';
}

function resetCode() {
    if (editor) {
        editor.setValue(originalCode);
        clearOutput();
    }
}

function copyOutput() {
    const outputDiv = document.getElementById('output');
    const text = outputDiv.innerText;
    navigator.clipboard.writeText(text).then(() => {
        const copyBtn = document.getElementById('copyBtn');
        const originalText = copyBtn.textContent;
        copyBtn.textContent = '✓ Copied!';
        setTimeout(() => {
            copyBtn.textContent = originalText;
        }, 2000);
    });
}

function loadExample(type) {
    const examples = {
        basic: `# Basic Python Calculations
print("=== Basic Calculations ===")

# Arithmetic operations
a, b = 10, 3
print(f"Addition: {a} + {b} = {a + b}")
print(f"Subtraction: {a} - {b} = {a - b}")
print(f"Multiplication: {a} * {b} = {a * b}")
print(f"Division: {a} / {b} = {a / b}")
print(f"Floor Division: {a} // {b} = {a // b}")
print(f"Modulus: {a} % {b} = {a % b}")
print(f"Power: {a} ** {b} = {a ** b}")

# List comprehension
squares = [x**2 for x in range(10)]
print(f"\\nSquares: {squares}")

# Dictionary
person = {"name": "Python", "age": 30, "language": "Python"}
print(f"\\nPerson: {person}")
`,

        numpy: `import numpy as np

print("=== NumPy Examples ===\\n")

# Array creation
arr1 = np.array([1, 2, 3, 4, 5])
print(f"Array: {arr1}")

# 2D array
arr2 = np.array([[1, 2, 3], [4, 5, 6]])
print(f"\\n2D Array:\\n{arr2}")

# Array operations
print(f"\\nSum: {np.sum(arr1)}")
print(f"Mean: {np.mean(arr1)}")
print(f"Std Dev: {np.std(arr1)}")
print(f"Min: {np.min(arr1)}")
print(f"Max: {np.max(arr1)}")

# Mathematical operations
angles = np.linspace(0, np.pi, 5)
print(f"\\nAngles: {angles}")
print(f"Sin values: {np.sin(angles)}")

# Random numbers
random_arr = np.random.randn(5)
print(f"\\nRandom normal distribution: {random_arr}")

# Linear algebra
matrix = np.array([[1, 2], [3, 4]])
print(f"\\nMatrix:\\n{matrix}")
print(f"Transpose:\\n{matrix.T}")
print(f"Inverse:\\n{np.linalg.inv(matrix)}")
`,

        matplotlib: `import numpy as np
import matplotlib.pyplot as plt

print("Creating plots...\\n")

# Create data
x = np.linspace(0, 10, 100)
y1 = np.sin(x)
y2 = np.cos(x)
y3 = np.exp(-x/5) * np.sin(x)

# First plot - Sine and Cosine
plt.figure(figsize=(10, 6))
plt.plot(x, y1, 'b-', label='sin(x)', linewidth=2)
plt.plot(x, y2, 'r-', label='cos(x)', linewidth=2)
plt.xlabel('x')
plt.ylabel('y')
plt.title('Sine and Cosine Functions')
plt.legend()
plt.grid(True, alpha=0.3)
plt.show()

# Second plot - Damped oscillation
plt.figure(figsize=(10, 4))
plt.plot(x, y3, 'g-', linewidth=2)
plt.xlabel('x')
plt.ylabel('y')
plt.title('Damped Oscillation')
plt.grid(True, alpha=0.3)
plt.show()

print("✓ Plots generated above!")
`,

        pandas: `import pandas as pd
import numpy as np

print("=== Pandas DataFrame Examples ===\\n")

# Create DataFrame
data = {
    'Name': ['Alice', 'Bob', 'Charlie', 'Diana', 'Eve'],
    'Age': [25, 30, 35, 28, 32],
    'Salary': [50000, 60000, 70000, 55000, 65000],
    'Department': ['Engineering', 'Sales', 'Engineering', 'HR', 'Sales']
}

df = pd.DataFrame(data)
print("DataFrame:")
print(df)

# DataFrame operations
print(f"\\nShape: {df.shape}")
print(f"\\nColumns: {df.columns.tolist()}")
print(f"\\nInfo:")
print(df.info())

# Statistics
print(f"\\nStatistics:")
print(df.describe())

# Group by
print(f"\\nAverage Salary by Department:")
print(df.groupby('Department')['Salary'].mean())

# Filtering
print(f"\\nPeople older than 30:")
print(df[df['Age'] > 30])

# Adding new column
df['Age_Group'] = pd.cut(df['Age'], bins=[20, 30, 40], labels=['20-30', '30-40'])
print(f"\\nDataFrame with Age Group:")
print(df)
`
    };
    
    if (examples[type] && editor) {
        editor.setValue(examples[type]);
        clearOutput();
    }
}

// Register service worker for offline capability
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.register('/service-worker.js').then(registration => {
        console.log('ServiceWorker registered');
    }).catch(error => {
        console.log('ServiceWorker registration failed:', error);
    });
}

// Initialize everything when page loads
window.addEventListener('load', async () => {
    initializeEditor();
    await initializePyodide();
});

// Make functions global for HTML buttons
window.runCode = runCode;
window.clearOutput = clearOutput;
window.resetCode = resetCode;
window.copyOutput = copyOutput;
window.loadExample = loadExample;
