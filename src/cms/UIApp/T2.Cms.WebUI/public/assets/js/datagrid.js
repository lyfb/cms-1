/* 此文件由系统自动生成! */
//
//文件：数据表格插件
//版本: 1.0
//时间：2011-10-01
//
function datagrid(h, k) { this.panel = h.nodeName ? h : jr.$(h); this.columns = k.columns; this.idField = k.idField || "id"; this.data_url = k.url; this.data = k.data; this.onLoaded = k.loaded; this.loadbox = null; this.gridView = null; this.loading = function () { if (this.gridView.offsetHeight == 0) { var a = this.gridView.previousSibling.offsetHeight; var b = this.panel.offsetHeight - this.gridView.previousSibling.offsetHeight; this.gridView.style.cssText = this.gridView.style.cssText.replace(/(\s*)height:[^;]+;/ig, ' height:' + (b > a ? b + 'px;' : 'auto')); var c = Math.ceil((this.gridView.clientWidth - this.loadbox.offsetWidth) / 2); var d = Math.ceil((this.gridView.clientHeight - this.loadbox.offsetHeight) / 2); this.loadbox.style.cssText = this.loadbox.style.cssText.replace(/(;\s*)*left:[^;]+;([\s\S]*)(\s)top:([^;]+)/ig, '$1left:' + c + 'px;$2 top:' + (d < 0 ? -d : d) + 'px') } this.loadbox.style.display = '' }; this._initLayout = function () { var a = ''; if (this.columns && this.columns.length != 0) { a += '<div class="ui-datagrid-header"><table width="100%" cellspacing="0" cellpadding="0"><tr>'; for (var i in this.columns) { a += '<td' + (i == 0 ? ' class="first"' : '') + (this.columns[i].align ? ' align="' + this.columns[i].align + '"' : '') + (this.columns[i].width ? ' width="' + this.columns[i].width + '"' : '') + '><div class="ui-datagrid-header-title">' + this.columns[i].title + '</div></td>' } a += '</tr></table></div>'; a += '<div class="ui-datagrid-view" style="position:relative;overflow:auto;height:0;">' + '<div class="loading" style="position: absolute; display: inline-block; left:0; top:0;">加载中...</div>' + '<div class="view"></div>' + '</div>' } this.panel.innerHTML = a; this.gridView = (this.panel.getElementsByClassName ? this.panel.getElementsByClassName('ui-datagrid-view') : jr.dom.getsByClass(this.panel, 'ui-datagrid-view'))[0]; this.loadbox = this.gridView.getElementsByTagName('DIV')[0] }; this._fill_data = function (a) { if (!a) return; var b; var c; var d; var e = ''; var f = a['rows'] || a; e += '<table width="100%" cellspacing="0" cellpadding="0">'; for (var i = 0; i < f.length; i++) { b = f[i]; e += '<tr' + (b[this.idField] ? ' data-indent="' + b[this.idField] + '"' : '') + '>'; for (var j in this.columns) { c = this.columns[j]; d = b[c.field]; e += '<td' + (j == 0 ? ' class="first"' : '') + (c.align ? ' align="' + c.align + '"' : '') + '><div style="' + (i == 0 && c.width ? ' width:' + c.width + 'px' : '') + '">' + (c.formatter && c.formatter instanceof Function ? c.formatter(d, b, i) : d) + '</div></td>' } e += '</tr>' } e += '</table><div style="clear:both"></div>'; var g = this.gridView.getElementsByTagName('DIV')[1]; g.innerHTML = e; g.srcollTop = 0; this.loadbox.style.display = 'none'; if (this.onLoaded && this.onLoaded instanceof Function) this.onLoaded(a) }; this._fixPosition = function () { }; this._load_data = function (b) { if (!this.data_url) return; var t = this; if (b) { if (!(b instanceof Function)) { b = null } } jr.xhr.request({ uri: this.data_url, data: 'json', params: this.data, method: 'POST' }, { success: function (a) { t._fill_data(a) }, error: function () { } }) }; this._initLayout(); this.load() } datagrid.prototype.resize = function () { this._fixPosition() }; datagrid.prototype.load = function (a) { this.loading(); if (a && a instanceof Object) { this._fill_data(a); return } this._load_data() }; datagrid.prototype.reload = function (a, b) { if (a) { this.data = a } this.load(b) }; jr.extend({ grid: function (a, b) { return new datagrid(a, b) }, datagrid: function (a, b) { return new datagrid(a, b) } });
