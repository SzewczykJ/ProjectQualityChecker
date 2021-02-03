/*!
 * Chart.js v3.0.0-alpha.2
 * https://www.chartjs.org
 * (c) 2020 Chart.js Contributors
 * Released under the MIT License
 */
import { isNullOrUndef, isArray } from './core.js';

const PI = Math.PI;
const RAD_PER_DEG = PI / 180;
const DOUBLE_PI = PI * 2;
const HALF_PI = PI / 2;
const QUARTER_PI = PI / 4;
const TWO_THIRDS_PI = PI * 2 / 3;
function toFontString(font) {
	if (!font || isNullOrUndef(font.size) || isNullOrUndef(font.family)) {
		return null;
	}
	return (font.style ? font.style + ' ' : '')
		+ (font.weight ? font.weight + ' ' : '')
		+ font.size + 'px '
		+ font.family;
}
function _measureText(ctx, data, gc, longest, string) {
	let textWidth = data[string];
	if (!textWidth) {
		textWidth = data[string] = ctx.measureText(string).width;
		gc.push(string);
	}
	if (textWidth > longest) {
		longest = textWidth;
	}
	return longest;
}
function _longestText(ctx, font, arrayOfThings, cache) {
	cache = cache || {};
	let data = cache.data = cache.data || {};
	let gc = cache.garbageCollect = cache.garbageCollect || [];
	if (cache.font !== font) {
		data = cache.data = {};
		gc = cache.garbageCollect = [];
		cache.font = font;
	}
	ctx.save();
	ctx.font = font;
	let longest = 0;
	const ilen = arrayOfThings.length;
	let i, j, jlen, thing, nestedThing;
	for (i = 0; i < ilen; i++) {
		thing = arrayOfThings[i];
		if (thing !== undefined && thing !== null && isArray(thing) !== true) {
			longest = _measureText(ctx, data, gc, longest, thing);
		} else if (isArray(thing)) {
			for (j = 0, jlen = thing.length; j < jlen; j++) {
				nestedThing = thing[j];
				if (nestedThing !== undefined && nestedThing !== null && !isArray(nestedThing)) {
					longest = _measureText(ctx, data, gc, longest, nestedThing);
				}
			}
		}
	}
	ctx.restore();
	const gcLen = gc.length / 2;
	if (gcLen > arrayOfThings.length) {
		for (i = 0; i < gcLen; i++) {
			delete data[gc[i]];
		}
		gc.splice(0, gcLen);
	}
	return longest;
}
function _alignPixel(chart, pixel, width) {
	const devicePixelRatio = chart.currentDevicePixelRatio;
	const halfWidth = width / 2;
	return Math.round((pixel - halfWidth) * devicePixelRatio) / devicePixelRatio + halfWidth;
}
function clear(chart) {
	chart.ctx.clearRect(0, 0, chart.width, chart.height);
}
function drawPoint(ctx, options, x, y) {
	let type, xOffset, yOffset, size, cornerRadius;
	const style = options.pointStyle;
	const rotation = options.rotation;
	const radius = options.radius;
	let rad = (rotation || 0) * RAD_PER_DEG;
	if (style && typeof style === 'object') {
		type = style.toString();
		if (type === '[object HTMLImageElement]' || type === '[object HTMLCanvasElement]') {
			ctx.save();
			ctx.translate(x, y);
			ctx.rotate(rad);
			ctx.drawImage(style, -style.width / 2, -style.height / 2, style.width, style.height);
			ctx.restore();
			return;
		}
	}
	if (isNaN(radius) || radius <= 0) {
		return;
	}
	ctx.beginPath();
	switch (style) {
	default:
		ctx.arc(x, y, radius, 0, DOUBLE_PI);
		ctx.closePath();
		break;
	case 'triangle':
		ctx.moveTo(x + Math.sin(rad) * radius, y - Math.cos(rad) * radius);
		rad += TWO_THIRDS_PI;
		ctx.lineTo(x + Math.sin(rad) * radius, y - Math.cos(rad) * radius);
		rad += TWO_THIRDS_PI;
		ctx.lineTo(x + Math.sin(rad) * radius, y - Math.cos(rad) * radius);
		ctx.closePath();
		break;
	case 'rectRounded':
		cornerRadius = radius * 0.516;
		size = radius - cornerRadius;
		xOffset = Math.cos(rad + QUARTER_PI) * size;
		yOffset = Math.sin(rad + QUARTER_PI) * size;
		ctx.arc(x - xOffset, y - yOffset, cornerRadius, rad - PI, rad - HALF_PI);
		ctx.arc(x + yOffset, y - xOffset, cornerRadius, rad - HALF_PI, rad);
		ctx.arc(x + xOffset, y + yOffset, cornerRadius, rad, rad + HALF_PI);
		ctx.arc(x - yOffset, y + xOffset, cornerRadius, rad + HALF_PI, rad + PI);
		ctx.closePath();
		break;
	case 'rect':
		if (!rotation) {
			size = Math.SQRT1_2 * radius;
			ctx.rect(x - size, y - size, 2 * size, 2 * size);
			break;
		}
		rad += QUARTER_PI;
	case 'rectRot':
		xOffset = Math.cos(rad) * radius;
		yOffset = Math.sin(rad) * radius;
		ctx.moveTo(x - xOffset, y - yOffset);
		ctx.lineTo(x + yOffset, y - xOffset);
		ctx.lineTo(x + xOffset, y + yOffset);
		ctx.lineTo(x - yOffset, y + xOffset);
		ctx.closePath();
		break;
	case 'crossRot':
		rad += QUARTER_PI;
	case 'cross':
		xOffset = Math.cos(rad) * radius;
		yOffset = Math.sin(rad) * radius;
		ctx.moveTo(x - xOffset, y - yOffset);
		ctx.lineTo(x + xOffset, y + yOffset);
		ctx.moveTo(x + yOffset, y - xOffset);
		ctx.lineTo(x - yOffset, y + xOffset);
		break;
	case 'star':
		xOffset = Math.cos(rad) * radius;
		yOffset = Math.sin(rad) * radius;
		ctx.moveTo(x - xOffset, y - yOffset);
		ctx.lineTo(x + xOffset, y + yOffset);
		ctx.moveTo(x + yOffset, y - xOffset);
		ctx.lineTo(x - yOffset, y + xOffset);
		rad += QUARTER_PI;
		xOffset = Math.cos(rad) * radius;
		yOffset = Math.sin(rad) * radius;
		ctx.moveTo(x - xOffset, y - yOffset);
		ctx.lineTo(x + xOffset, y + yOffset);
		ctx.moveTo(x + yOffset, y - xOffset);
		ctx.lineTo(x - yOffset, y + xOffset);
		break;
	case 'line':
		xOffset = Math.cos(rad) * radius;
		yOffset = Math.sin(rad) * radius;
		ctx.moveTo(x - xOffset, y - yOffset);
		ctx.lineTo(x + xOffset, y + yOffset);
		break;
	case 'dash':
		ctx.moveTo(x, y);
		ctx.lineTo(x + Math.cos(rad) * radius, y + Math.sin(rad) * radius);
		break;
	}
	ctx.fill();
	if (options.borderWidth > 0) {
		ctx.stroke();
	}
}
function _isPointInArea(point, area) {
	const epsilon = 0.5;
	return point.x > area.left - epsilon && point.x < area.right + epsilon &&
		point.y > area.top - epsilon && point.y < area.bottom + epsilon;
}
function clipArea(ctx, area) {
	ctx.save();
	ctx.beginPath();
	ctx.rect(area.left, area.top, area.right - area.left, area.bottom - area.top);
	ctx.clip();
}
function unclipArea(ctx) {
	ctx.restore();
}
function _steppedLineTo(ctx, previous, target, flip, mode) {
	if (!previous) {
		return ctx.lineTo(target.x, target.y);
	}
	if (mode === 'middle') {
		const midpoint = (previous.x + target.x) / 2.0;
		ctx.lineTo(midpoint, previous.y);
		ctx.lineTo(midpoint, target.y);
	} else if (mode === 'after' !== !!flip) {
		ctx.lineTo(previous.x, target.y);
	} else {
		ctx.lineTo(target.x, previous.y);
	}
	ctx.lineTo(target.x, target.y);
}
function _bezierCurveTo(ctx, previous, target, flip) {
	if (!previous) {
		return ctx.lineTo(target.x, target.y);
	}
	ctx.bezierCurveTo(
		flip ? previous.controlPointPreviousX : previous.controlPointNextX,
		flip ? previous.controlPointPreviousY : previous.controlPointNextY,
		flip ? target.controlPointNextX : target.controlPointPreviousX,
		flip ? target.controlPointNextY : target.controlPointPreviousY,
		target.x,
		target.y);
}

export { _alignPixel, _bezierCurveTo, _isPointInArea, _longestText, _measureText, _steppedLineTo, clear, clipArea, drawPoint, toFontString, unclipArea };
