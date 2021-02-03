/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
function noop() {}
const uid = (function() {
	let id = 0;
	return function() {
		return id++;
	};
}());
function isNullOrUndef(value) {
	return value === null || typeof value === 'undefined';
}
function isArray(value) {
	if (Array.isArray && Array.isArray(value)) {
		return true;
	}
	const type = Object.prototype.toString.call(value);
	if (type.substr(0, 7) === '[object' && type.substr(-6) === 'Array]') {
		return true;
	}
	return false;
}
function isObject(value) {
	return value !== null && Object.prototype.toString.call(value) === '[object Object]';
}
const isNumberFinite = (value) => (typeof value === 'number' || value instanceof Number) && isFinite(+value);
function valueOrDefault(value, defaultValue) {
	return typeof value === 'undefined' ? defaultValue : value;
}
function callback(fn, args, thisArg) {
	if (fn && typeof fn.call === 'function') {
		return fn.apply(thisArg, args);
	}
}
function each(loopable, fn, thisArg, reverse) {
	let i, len, keys;
	if (isArray(loopable)) {
		len = loopable.length;
		if (reverse) {
			for (i = len - 1; i >= 0; i--) {
				fn.call(thisArg, loopable[i], i);
			}
		} else {
			for (i = 0; i < len; i++) {
				fn.call(thisArg, loopable[i], i);
			}
		}
	} else if (isObject(loopable)) {
		keys = Object.keys(loopable);
		len = keys.length;
		for (i = 0; i < len; i++) {
			fn.call(thisArg, loopable[keys[i]], keys[i]);
		}
	}
}
function _elementsEqual(a0, a1) {
	let i, ilen, v0, v1;
	if (!a0 || !a1 || a0.length !== a1.length) {
		return false;
	}
	for (i = 0, ilen = a0.length; i < ilen; ++i) {
		v0 = a0[i];
		v1 = a1[i];
		if (v0.datasetIndex !== v1.datasetIndex || v0.index !== v1.index) {
			return false;
		}
	}
	return true;
}
function clone(source) {
	if (isArray(source)) {
		return source.map(clone);
	}
	if (isObject(source)) {
		const target = Object.create(source);
		const keys = Object.keys(source);
		const klen = keys.length;
		let k = 0;
		for (; k < klen; ++k) {
			target[keys[k]] = clone(source[keys[k]]);
		}
		return target;
	}
	return source;
}
function _merger(key, target, source, options) {
	const tval = target[key];
	const sval = source[key];
	if (isObject(tval) && isObject(sval)) {
		merge(tval, sval, options);
	} else {
		target[key] = clone(sval);
	}
}
function merge(target, source, options) {
	const sources = isArray(source) ? source : [source];
	const ilen = sources.length;
	if (!isObject(target)) {
		return target;
	}
	options = options || {};
	const merger = options.merger || _merger;
	for (let i = 0; i < ilen; ++i) {
		source = sources[i];
		if (!isObject(source)) {
			continue;
		}
		const keys = Object.keys(source);
		for (let k = 0, klen = keys.length; k < klen; ++k) {
			merger(keys[k], target, source, options);
		}
	}
	return target;
}
function mergeIf(target, source) {
	return merge(target, source, {merger: _mergerIf});
}
function _mergerIf(key, target, source) {
	const tval = target[key];
	const sval = source[key];
	if (isObject(tval) && isObject(sval)) {
		mergeIf(tval, sval);
	} else if (!Object.prototype.hasOwnProperty.call(target, key)) {
		target[key] = clone(sval);
	}
}
function _deprecated(scope, value, previous, current) {
	if (value !== undefined) {
		console.warn(scope + ': "' + previous +
			'" is deprecated. Please use "' + current + '" instead');
	}
}
function resolveObjectKey(obj, key) {
	if (key.length < 3) {
		return obj[key];
	}
	const keys = key.split('.');
	for (let i = 0, n = keys.length; i < n; ++i) {
		const k = keys[i];
		if (k in obj) {
			obj = obj[k];
		} else {
			return;
		}
	}
	return obj;
}
function _capitalize(str) {
	return str.charAt(0).toUpperCase() + str.slice(1);
}

export { _capitalize, _deprecated, _elementsEqual, _merger, _mergerIf, callback, clone, each, isArray, isNumberFinite as isFinite, isNullOrUndef, isObject, merge, mergeIf, noop, resolveObjectKey, uid, valueOrDefault };
