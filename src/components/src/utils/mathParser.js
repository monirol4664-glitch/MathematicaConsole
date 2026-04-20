import { evaluate } from 'mathjs';

export function evaluateFunction(funcStr, x) {
  try {
    // Replace ^ with ** for mathjs
    let expression = funcStr.replace(/\^/g, '**');
    
    // Handle implicit multiplication (e.g., "2x" -> "2*x")
    expression = expression.replace(/(\d)([a-z])/gi, '$1*$2');
    expression = expression.replace(/([a-z])(\d)/gi, '$1*$2');
    expression = expression.replace(/\)\(/g, ')*(');
    
    // Add missing multiplication signs
    expression = expression.replace(/(\d)\(/g, '$1*(');
    expression = expression.replace(/\)(\d)/g, ')*$1');
    expression = expression.replace(/([a-z])\(/gi, '$1*(');
    
    // Evaluate using mathjs with x parameter
    const result = evaluate(expression, { x: x });
    
    return result;
  } catch (error) {
    console.error('Evaluation error:', error);
    return NaN;
  }
}

export function evaluateExpression(expr, variables = {}) {
  try {
   