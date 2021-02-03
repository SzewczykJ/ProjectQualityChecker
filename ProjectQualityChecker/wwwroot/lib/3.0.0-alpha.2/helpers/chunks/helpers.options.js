/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
import { merge, valueOrDefault, isObject, isArray } from '../core.js';
import { toFontString } from '../canvas.js';

function getScope(node, key) {
	if (!key) {
		return node;
	}
	const keys = key.split('.');
	for (let i = 0, n = keys.length; i < n; ++i) {
		const k = keys[i];
		node = node[k] || (node[k] = {});
	}
	return node;
}
class Defaults {
	constructor() {
		this.color = 'rgba(0,0,0,0.1)';
		this.elements = {};
		this.events = [
			'mousemove',
			'mouseout',
			'click',
			'touchstart',
			'touchmove'
		];
		this.font = {
			color: '#666',
			family: "'Helvetica Neue', 'Helvetica', 'Arial', sans-serif",
			size: 12,
			style: 'normal',
			lineHeight: 1.2,
			weight: null,
			lineWidth: 0,
			strokeStyle: undefined
		};
		this.hover = {
			onHover: null,
			mode: 'nearest',
			intersect: true
		};
		this.maintainAspectRatio = true;
		this.onClick = null;
		this.responsive = true;
		this.showLines = true;
		this.plugins = {};
		this.scale = undefined;
		this.doughnut = undefined;
		this.scales = {};
		this.controllers = undefined;
	}
	set(scope, values) {
		return merge(getScope(this, scope), values);
	}
	get(scope) {
		return getScope(this, scope);
	}
	route(scope, name, targetScope, targetName) {
		const scopeObject = getScope(this, scope);
		const targetScopeObject = getScope(this, targetScope);
		const privateName = '_' + name;
		Object.defineProperties(scopeObject, {
			[privateName]: {
				writable: true
			},
			[name]: {
				enumerable: true,
				get() {
					return valueOrDefault(this[privateName], targetScopeObject[targetName]);
				},
				set(value) {
					this[privateName] = value;
				}
			}
		});
	}
}
var defaults = new Defaults();

function toLineHeight(value, size) {
	const matches = ('' + value).match(/^(normal|(\d+(?:\.\d+)?)(px|em|%)?)$/);
	if (!matches || matches[1] === 'normal') {
		return size * 1.2;
	}
	value = +matches[2];
	switch (matches[3]) {
	case 'px':
		return value;
	case '%':
		value /= 100;
		break;
	}
	return size * value;
}
function toPadding(value) {
	let t, r, b, l;
	if (isObject(value)) {
		t = +value.top || 0;
		r = +value.right || 0;
		b = +value.bottom || 0;
		l = +value.left || 0;
	} else {
		t = r = b = l = +value || 0;
	}
	return {
		top: t,
		right: r,
		bottom: b,
		left: l,
		height: t + b,
		width: l + r
	};
}
function toFont(options) {
	const defaultFont = defaults.font;
	options = options || {};
	let size = valueOrDefault(options.size, defaultFont.size);
	if (typeof size === 'string') {
		size = parseInt(size, 10);
	}
	const font = {
		color: valueOrDefault(options.color, defaultFont.color),
		family: valueOrDefault(options.family, defaultFont.family),
		lineHeight: toLineHeight(valueOrDefault(options.lineHeight, defaultFont.lineHeight), size),
		lineWidth: valueOrDefault(options.lineWidth, defaultFont.lineWidth),
		size,
		style: valueOrDefault(options.style, defaultFont.style),
		weight: valueOrDefault(options.weight, defaultFont.weight),
		strokeStyle: valueOrDefault(options.strokeStyle, defaultFont.strokeStyle),
		string: ''
	};
	font.string = toFontString(font);
	return font;
}
function resolve(inputs, context, index, info) {
	let cacheable = true;
	let i, ilen, value;
	for (i = 0, ilen = inputs.length; i < ilen; ++i) {
		value = inputs[i];
		if (value === undefined) {
			continue;
		}
		if (context !== undefined && typeof value === 'function') {
			value = value(context);
			cacheable = false;
		}
		if (index !== undefined && isArray(value)) {
			value = value[index % value.length];
			cacheable = false;
		}
		if (value !== undefined) {
			if (info && !cacheable) {
				info.cacheable = false;
			}
			return value;
		}
	}
}

export { toFont as a, toLineHeight as b, defaults as d, resolve as r, toPadding as t };
