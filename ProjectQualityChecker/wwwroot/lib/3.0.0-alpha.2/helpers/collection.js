/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
import { _capitalize } from './core.js';

function _lookup(table, value) {
	let hi = table.length - 1;
	let lo = 0;
	let mid;
	while (hi - lo > 1) {
		mid = (lo + hi) >> 1;
		if (table[mid] < value) {
			lo = mid;
		} else {
			hi = mid;
		}
	}
	return {lo, hi};
}
function _lookupByKey(table, key, value) {
	let hi = table.length - 1;
	let lo = 0;
	let mid;
	while (hi - lo > 1) {
		mid = (lo + hi) >> 1;
		if (table[mid][key] < value) {
			lo = mid;
		} else {
			hi = mid;
		}
	}
	return {lo, hi};
}
function _rlookupByKey(table, key, value) {
	let hi = table.length - 1;
	let lo = 0;
	let mid;
	while (hi - lo > 1) {
		mid = (lo + hi) >> 1;
		if (table[mid][key] < value) {
			hi = mid;
		} else {
			lo = mid;
		}
	}
	return {lo, hi};
}
function _filterBetween(values, min, max) {
	let start = 0;
	let end = values.length;
	while (start < end && values[start] < min) {
		start++;
	}
	while (end > start && values[end - 1] > max) {
		end--;
	}
	return start > 0 || end < values.length
		? values.slice(start, end)
		: values;
}
const arrayEvents = ['push', 'pop', 'shift', 'splice', 'unshift'];
function listenArrayEvents(array, listener) {
	if (array._chartjs) {
		array._chartjs.listeners.push(listener);
		return;
	}
	Object.defineProperty(array, '_chartjs', {
		configurable: true,
		enumerable: false,
		value: {
			listeners: [listener]
		}
	});
	arrayEvents.forEach((key) => {
		const method = '_onData' + _capitalize(key);
		const base = array[key];
		Object.defineProperty(array, key, {
			configurable: true,
			enumerable: false,
			value(...args) {
				const res = base.apply(this, args);
				array._chartjs.listeners.forEach((object) => {
					if (typeof object[method] === 'function') {
						object[method](...args);
					}
				});
				return res;
			}
		});
	});
}
function unlistenArrayEvents(array, listener) {
	const stub = array._chartjs;
	if (!stub) {
		return;
	}
	const listeners = stub.listeners;
	const index = listeners.indexOf(listener);
	if (index !== -1) {
		listeners.splice(index, 1);
	}
	if (listeners.length > 0) {
		return;
	}
	arrayEvents.forEach((key) => {
		delete array[key];
	});
	delete array._chartjs;
}
function _arrayUnique(items) {
	const set = new Set();
	let i, ilen;
	for (i = 0, ilen = items.length; i < ilen; ++i) {
		set.add(items[i]);
	}
	if (set.size === ilen) {
		return items;
	}
	const result = [];
	set.forEach(item => {
		result.push(item);
	});
	return result;
}

export { _arrayUnique, _filterBetween, _lookup, _lookupByKey, _rlookupByKey, listenArrayEvents, unlistenArrayEvents };
