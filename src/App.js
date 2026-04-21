limport React, { useState } from 'react';

function App() {
  const [equation, setEquation] = useState('2x + 3 = 7');
  const [solution, setSolution] = useState(null);
  const [error, setError] = useState(null);

  const solveEquation = () => {
    setError(null);
    try {
      let eq = equation.replace(/\s/g, '');
      let sides = eq.split('=');
      
      if (sides.length !== 2) {
        setError('Invalid equation format');
        return;
      }
      
      let left = sides[0];
      let right = sides[1];
      
      // Parse left side
      let leftX = 0, leftConst = 0;
      let leftTerms = left.split(/(?=[+-])/);
      leftTerms.forEach(term => {
        if (term.includes('x')) {
          let coeff = term.replace('x', '');
          if (coeff === '+' || coeff === '') coeff = '1';
          if (coeff === '-') coeff = '-1';
          leftX += eval(coeff);
        } else if (term) {
          leftConst += eval(term);
        }
      });
      
      // Parse right side
      let rightX = 0, rightConst = 0;
      let rightTerms = right.split(/(?=[+-])/);
      rightTerms.forEach(term => {
        if (term.includes('x')) {
          let coeff = term.replace('x', '');
          if (coeff === '+' || coeff === '') coeff = '1';
          if (coeff === '-') coeff = '-1';
          rightX += eval(coeff);
        } else if (term) {
          rightConst += eval(term);
        }
      });
      
      // Solve ax + b = 0
      let a = leftX - rightX;
      let b = leftConst - rightConst;
      
      if (a === 0) {
        setError(b === 0 ? 'Infinite solutions' : 'No solution');
        return;
      }
      
      let x = -b / a;
      setSolution(x);
      
    } catch(e) {
      setError('Invalid equation');
      setSolution(null);
    }
  };

  const setExample = (eq) => {
    setEquation(eq);
    setSolution(null);
    setError(null);
  };

  return (
    <div style={{padding: 20, maxWidth: 500, margin: '0 auto', fontFamily: 'Arial'}}>
      <h1 style={{textAlign: 'center'}}>📐 Math Solver</h1>
      
      <div style={{marginBottom: 20}}>
        <button onClick={() => setExample('2x + 3 = 7')} style={{margin: 5, padding: 8}}>2x+3=7</button>
        <button onClick={() => setExample('x/2 + 1/3 = 3x/4')} style={{margin: 5, padding: 8}}>With Fractions</button>
        <button onClick={() => setExample('5x - 2 = 3x + 8')} style={{margin: 5, padding: 8}}>5x-2=3x+8</button>
      </div>
      
      <input 
        value={equation} 
        onChange={(e) => setEquation(e.target.value)}
        style={{width: '100%', padding: 12, fontSize: 16, marginBottom: 10}}
        placeholder="Enter equation"
      />
      
      <button 
        onClick={solveEquation} 
        style={{width: '100%', padding: 12, fontSize: 16, background: '#4F46E5', color: 'white', border: 'none', borderRadius: 8}}
      >
        Solve Equation
      </button>
      
      {error && (
        <div style={{marginTop: 20, padding: 20, background: '#FEE2E2', color: '#DC2626', borderRadius: 8}}>
          Error: {error}
        </div>
      )}
      
      {solution !== null && (
        <div style={{marginTop: 20, padding: 20, background: '#D1FAE5', borderRadius: 8}}>
          <div style={{fontSize: 14, color: '#059669'}}>✓ Solution Found</div>
          <div style={{fontSize: 32, fontWeight: 'bold', marginTop: 10}}>
            x = {solution}
          </div>
          <div style={{marginTop: 10, fontSize: 12, color: '#666'}}>
            As fraction: {Math.round(solution * 100) / 100}
          </div>
        </div>
      )}
      
      <div style={{marginTop: 20, padding: 16, background: '#F3F4F6', borderRadius: 8, fontSize: 12}}>
        <strong>Supported formats:</strong><br/>
        • 2x + 3 = 7<br/>
        • x/2 + 1/3 = 3x/4<br/>
        • 5x - 2 = 3x + 8
      </div>
    </div>
  );
}

export default App;