/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
import './core.js';
import { _angleBetween, _angleDiff, _normalizeAngle } from './math.js';

function propertyFn(property) {
	if (property === 'angle') {
		return {
			between: _angleBetween,
			compare: _angleDiff,
			normalize: _normalizeAngle,
		};
	}
	return {
		between: (n, s, e) => n >= s && n <= e,
		compare: (a, b) => a - b,
		normalize: x => x
	};
}
function makeSubSegment(start, end, loop, count) {
	return {
		start: start % count,
		end: end % count,
		loop: loop && (end - start + 1) % count === 0
	};
}
function getSegment(segment, points, bounds) {
	const {property, start: startBound, end: endBound} = bounds;
	const {between, normalize} = propertyFn(property);
	const count = points.length;
	let {start, end, loop} = segment;
	let i, ilen;
	if (loop) {
		start += count;
		end += count;
		for (i = 0, ilen = count; i < ilen; ++i) {
			if (!between(normalize(points[start % count][property]), startBound, endBound)) {
				break;
			}
			start--;
			end--;
		}
		start %= count;
		end %= count;
	}
	if (end < start) {
		end += count;
	}
	return {start, end, loop};
}
function _boundSegment(segment, points, bounds) {
	if (!bounds) {
		return [segment];
	}
	const {property, start: startBound, end: endBound} = bounds;
	const count = points.length;
	const {compare, between, normalize} = propertyFn(property);
	const {start, end, loop} = getSegment(segment, points, bounds);
	const result = [];
	let inside = false;
	let subStart = null;
	let i, value, point, prev;
	for (i = start; i <= end; ++i) {
		point = points[i % count];
		if (point.skip) {
			continue;
		}
		value = normalize(point[property]);
		inside = between(value, startBound, endBound);
		if (subStart === null && inside) {
			subStart = i > start && compare(value, startBound) > 0 ? prev : i;
		}
		if (subStart !== null && (!inside || compare(value, endBound) === 0)) {
			result.push(makeSubSegment(subStart, i, loop, count));
			subStart = null;
		}
		prev = i;
	}
	if (subStart !== null) {
		result.push(makeSubSegment(subStart, end, loop, count));
	}
	return result;
}
function _boundSegments(line, bounds) {
	const result = [];
	const segments = line.segments;
	for (let i = 0; i < segments.length; i++) {
		const sub = _boundSegment(segments[i], line.points, bounds);
		if (sub.length) {
			result.push(...sub);
		}
	}
	return result;
}
function findStartAndEnd(points, count, loop, spanGaps) {
	let start = 0;
	let end = count - 1;
	if (loop && !spanGaps) {
		while (start < count && !points[start].skip) {
			start++;
		}
	}
	while (start < count && points[start].skip) {
		start++;
	}
	start %= count;
	if (loop) {
		end += start;
	}
	while (end > start && points[end % count].skip) {
		end--;
	}
	end %= count;
	return {start, end};
}
function solidSegments(points, start, max, loop) {
	const count = points.length;
	const result = [];
	let last = start;
	let prev = points[start];
	let end;
	for (end = start + 1; end <= max; ++end) {
		const cur = points[end % count];
		if (cur.skip || cur.stop) {
			if (!prev.skip) {
				loop = false;
				result.push({start: start % count, end: (end - 1) % count, loop});
				start = last = cur.stop ? end : null;
			}
		} else {
			last = end;
			if (prev.skip) {
				start = end;
			}
		}
		prev = cur;
	}
	if (last !== null) {
		result.push({start: start % count, end: last % count, loop});
	}
	return result;
}
function _computeSegments(line) {
	const points = line.points;
	const spanGaps = line.options.spanGaps;
	const count = points.length;
	if (!count) {
		return [];
	}
	const loop = !!line._loop;
	const {start, end} = findStartAndEnd(points, count, loop, spanGaps);
	if (spanGaps === true) {
		return [{start, end, loop}];
	}
	const max = end < start ? end + count : end;
	const completeLoop = !!line._fullLoop && start === 0 && end === count - 1;
	return solidSegments(points, start, max, completeLoop);
}

export { _boundSegment, _boundSegments, _computeSegments };
