/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
import { isFinite as isNumberFinite } from './core.js';

const PI = Math.PI;
const TAU = 2 * PI;
const PITAU = TAU + PI;
function _factorize(value) {
	const result = [];
	const sqrt = Math.sqrt(value);
	let i;
	for (i = 1; i < sqrt; i++) {
		if (value % i === 0) {
			result.push(i);
			result.push(value / i);
		}
	}
	if (sqrt === (sqrt | 0)) {
		result.push(sqrt);
	}
	result.sort((a, b) => a - b).pop();
	return result;
}
const log10 = Math.log10 || function(x) {
	const exponent = Math.log(x) * Math.LOG10E;
	const powerOf10 = Math.round(exponent);
	const isPowerOf10 = x === Math.pow(10, powerOf10);
	return isPowerOf10 ? powerOf10 : exponent;
};
function isNumber(n) {
	return !isNaN(parseFloat(n)) && isFinite(n);
}
function almostEquals(x, y, epsilon) {
	return Math.abs(x - y) < epsilon;
}
function almostWhole(x, epsilon) {
	const rounded = Math.round(x);
	return ((rounded - epsilon) <= x) && ((rounded + epsilon) >= x);
}
function _setMinAndMaxByKey(array, target, property) {
	let i, ilen, value;
	for (i = 0, ilen = array.length; i < ilen; i++) {
		value = array[i][property];
		if (!isNaN(value)) {
			target.min = Math.min(target.min, value);
			target.max = Math.max(target.max, value);
		}
	}
}
const sign = Math.sign ?
	function(x) {
		return Math.sign(x);
	} :
	function(x) {
		x = +x;
		if (x === 0 || isNaN(x)) {
			return x;
		}
		return x > 0 ? 1 : -1;
	};
function toRadians(degrees) {
	return degrees * (PI / 180);
}
function toDegrees(radians) {
	return radians * (180 / PI);
}
function _decimalPlaces(x) {
	if (!isNumberFinite(x)) {
		return;
	}
	let e = 1;
	let p = 0;
	while (Math.round(x * e) / e !== x) {
		e *= 10;
		p++;
	}
	return p;
}
function getAngleFromPoint(centrePoint, anglePoint) {
	const distanceFromXCenter = anglePoint.x - centrePoint.x;
	const distanceFromYCenter = anglePoint.y - centrePoint.y;
	const radialDistanceFromCenter = Math.sqrt(distanceFromXCenter * distanceFromXCenter + distanceFromYCenter * distanceFromYCenter);
	let angle = Math.atan2(distanceFromYCenter, distanceFromXCenter);
	if (angle < (-0.5 * PI)) {
		angle += TAU;
	}
	return {
		angle,
		distance: radialDistanceFromCenter
	};
}
function distanceBetweenPoints(pt1, pt2) {
	return Math.sqrt(Math.pow(pt2.x - pt1.x, 2) + Math.pow(pt2.y - pt1.y, 2));
}
function _angleDiff(a, b) {
	return (a - b + PITAU) % TAU - PI;
}
function _normalizeAngle(a) {
	return (a % TAU + TAU) % TAU;
}
function _angleBetween(angle, start, end) {
	const a = _normalizeAngle(angle);
	const s = _normalizeAngle(start);
	const e = _normalizeAngle(end);
	const angleToStart = _normalizeAngle(s - a);
	const angleToEnd = _normalizeAngle(e - a);
	const startToAngle = _normalizeAngle(a - s);
	const endToAngle = _normalizeAngle(a - e);
	return a === s || a === e || (angleToStart > angleToEnd && startToAngle < endToAngle);
}
function _limitValue(value, min, max) {
	return Math.max(min, Math.min(max, value));
}

export { _angleBetween, _angleDiff, _decimalPlaces, _factorize, _limitValue, _normalizeAngle, _setMinAndMaxByKey, almostEquals, almostWhole, distanceBetweenPoints, getAngleFromPoint, isNumber, log10, sign, toDegrees, toRadians };
