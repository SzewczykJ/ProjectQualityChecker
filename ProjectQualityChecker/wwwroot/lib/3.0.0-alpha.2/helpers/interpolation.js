/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
function _pointInLine(p1, p2, t, mode) {
	return {
		x: p1.x + t * (p2.x - p1.x),
		y: p1.y + t * (p2.y - p1.y)
	};
}
function _steppedInterpolation(p1, p2, t, mode) {
	return {
		x: p1.x + t * (p2.x - p1.x),
		y: mode === 'middle' ? t < 0.5 ? p1.y : p2.y
		: mode === 'after' ? t < 1 ? p1.y : p2.y
		: t > 0 ? p2.y : p1.y
	};
}
function _bezierInterpolation(p1, p2, t, mode) {
	const cp1 = {x: p1.controlPointNextX, y: p1.controlPointNextY};
	const cp2 = {x: p2.controlPointPreviousX, y: p2.controlPointPreviousY};
	const a = _pointInLine(p1, cp1, t);
	const b = _pointInLine(cp1, cp2, t);
	const c = _pointInLine(cp2, p2, t);
	const d = _pointInLine(a, b, t);
	const e = _pointInLine(b, c, t);
	return _pointInLine(d, e, t);
}

export { _bezierInterpolation, _pointInLine, _steppedInterpolation };
