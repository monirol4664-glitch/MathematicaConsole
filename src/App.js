import React, { useState } from 'react';

function App() {
  const [equation, setEquation] = useState('2x+3=7');
  const [solution, setSolution] = useState(null);
  const [error, setError] = useState(null);

  const solveEquation = () => {
    setError(null);
    setSolution(null);
    
    try {
      let eq = equation.replace(/\s/g, '');
      let sides = eq.split('=');
      
      if (sides.length !== 2) {
        setError('Use format: expression = expression');
        return;
      }
      
      function parseExpression(expr) {
        let xCoeff = 0;
        let constant = 0;
        
        // Split into terms
        let terms = [];
        let currentTerm = '';
        for (let i = 0; i < expr.length; i++) {
          let char = expr[i];
          if ((char === '+' || char === '-') && currentTerm !== '') {
            terms.push(currentTerm);
            currentTerm = char;
          } else {
            currentTerm += char;
          }
        }
        if (currentTerm !== '') terms.push(currentTerm);
        
        for (let term of terms) {
          if (term === '') continue;
          
          if (term.includes('x')) {
            let coeff = term.replace('x', '');
            if (coeff === '+' || coeff === '') coeff = '1';
            if (coeff === '-') coeff = '-1';
            xCoeff += parseFloat(coeff);
          } else {
            constant += parseFloat(term);
          }
        }
        
        return { xCoeff, constant };
      }
      
      let left = parseExpression(sides[0]);
      let right = parseExpression(sides[1]);
      
      let a = left.xCoeff - right.xCoeff;
      let b = left.constant - right.constant;
      
      if (Math.abs(a) < 0.000001) {
        setError(Math.abs(b) < 0.000001 ? 'Infinite solutions' : 'No solution');
        return;
      }
      
      let x = -b / a;
      setSolution(x);
      
    } catch(e) {
      setError('Invalid equation. Use format like: 2x+3=7');
    }
  };

  const examples = ['2x+3=7', 'x/2+1/3=3x/4', '5x-2=3x+8', 'x+5=10'];

  return (
    <div style={{
      minHeight: '100vh',
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      padding: '20px',
      fontFamily: 'Arial, sans-serif'
    }}>
      <div style={{
        maxWidth: '500px',
        margin: '0 auto',
        background: 'white',
        borderRadius: '20px',
        padding: '30px',
        boxShadow: '0 20px 60px rgba(0,0,0,0.3)'
      }}>
        <h1 style={{
          textAlign: 'center',
          fontSize: '28px',
          marginBottom: '10px',
          color: '#1F2937'
        }}>
          📐 Math Solver
        </h1>
        
        <p style={{
          textAlign: 'center',
          color: '#6B7280',
          marginBottom: '30px'
        }}>
          Linear Equation Solver
        </p>
        
        <div style={{
          display: 'flex',
          gap: '10px',
          flexWrap: 'wrap',
          marginBottom: '20px',
          justifyContent: 'center'
        }}>
          {examples.map(eq => (
            <button
              key={eq}
              onClick={() => setEquation(eq)}
              style={{
                padding: '8px 16px',
                background: '#F3F4F6',
                border: 'none',
                borderRadius: '20px',
                cursor: 'pointer',
                fontSize: '14px'
              }}
            >
              {eq}
            </button>
          ))}
        </div>
        
        <input
          type="text"
          value={equation}
          onChange={(e) => setEquation(e.target.value)}
          style={{
            width: '100%',
            padding: '12px',
            fontSize: '16px',
            border: '2px solid #E5E7EB',
            borderRadius: '12px',
            marginBottom: '20px',
            fontFamily: 'monospace'
          }}
          placeholder="Enter equation..."
        />
        
        <button
          onClick={solveEquation}
          style={{
            width: '100%',
            padding: '14px',
            background: '#4F46E5',
            color: 'white',
            border: 'none',
            borderRadius: '12px',
            fontSize: '16px',
            fontWeight: 'bold',
            cursor: 'pointer'
          }}
        >
          Solve Equation
        </button>
        
        {error && (
          <div style={{
            marginTop: '20px',
            padding: '15px',
            background: '#FEE2E2',
            color: '#DC2626',
            borderRadius: '12px'
          }}>
            ⚠️ {error}
          </div>
        )}
        
        {solution !== null && (
          <div style={{
            marginTop: '20px',
            padding: '20px',
            background: '#D1FAE5',
            borderRadius: '12px',
            textAlign: 'center'
          }}>
            <div style={{
              fontSize: '14px',
              color: '#059669',
              marginBottom: '10px'
            }}>
              ✓ Solution Found
            </div>
            <div style={{
              fontSize: '36px',
              fontWeight: 'bold',
              color: '#1F2937',
              fontFamily: 'monospace'
            }}>
              x = {solution.toFixed(4)}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;