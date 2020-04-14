
/**
 * Created by ZhangKai on 2016/7/24.
 */
; (function (global, $, doc) {
    'use strict';

    var kaiBanner = (function () {

        function KaiBanner(el, config) {
            this.$el = $(el);
            this.initializeEles();
            this.setConfig(config);
            this.eventsMap = {
                'click .kai_banner_bottombtns': 'bottomBtnsClickHandle',
                'click .kai_banner_prevbtn': 'prevBtnClickHandle',
                'click .kai_banner_nextbtn': 'nextBtnClickHandle'
            };
            this.init();
        }

        KaiBanner.Eles = {
            $bannerBody: '.kai_banner_body',
            $bottomBtns: '.kai_banner_bottombtns',
            $prevBtn: '.kai_banner_prevbtn',
            $nextBtn: '.kai_banner_nextbtn'
        };

        var utils = {
            throttle: function (callback, delayTime, maxTime) {
                var timer = null;
                var prevTime = 0;
                // other
                return function () {
                    var context = this;
                    var argument = arguments;
                    var currTime = Date.now();
                    if (maxTime && currTime - prevTime >= maxTime) {
                        prevTime = currTime;
                        callback.apply(context, argument);
                    } else {
                        if (timer) clearTimeout(timer);
                        timer = setTimeout(function () {
                            callback.apply(context, argument);
                        }, delayTime);
                    }
                };
            },
            isObject: function (arg) {
                return Object.prototype.toString.call(arg) == '[object Object]';
            }
        };

        KaiBanner.prototype.initializeEles = function () {
            var eles = KaiBanner.Eles;
            var $node = null;
            for (var name in eles) {
                if (eles.hasOwnProperty(name)) {
                    $node = this.$el.find(eles[name]);
                    this[name] = $node.length > 0 ? $node : null;
                }
            }
        };

        KaiBanner.prototype.setConfig = function (config) {
            var _ = this;
            _.config = {
                speed: 1000,
                fixedWidth: false,
                intervalTime: 3000,
                highlightClass: ''
            };

            if (utils.isObject(config)) {
                $.extend(_.config, config);
            }
            _.distance = parseInt(_.$el.css('width'));
            _.maxIndex = _.$bannerBody.children().length - 1;
            _.direction = 'toRight';
            _.currentIndex = 0;
            _.timer = null;
        };

        KaiBanner.prototype.animate = function (speed) {
            var _ = this;

            _.$bannerBody
                .stop()
                .animate({ 'left': -(_.currentIndex * _.distance) + 'px' }, speed);
            if (_.$bottomBtns.children().length > 10 && _.$bottomBtns.children().length - _.currentIndex > 10) {
                _.$bottomBtns
                    .stop()
                    .animate({ 'left': -(_.currentIndex * 120) + 'px' }, speed)
            }

            if (_.$bottomBtns) {
                _.$bottomBtns
                    .children()
                    .eq(_.currentIndex)
                    .addClass(_.config.highlightClass)
                    .siblings()
                    .removeClass(_.config.highlightClass);
            }
        };

        KaiBanner.prototype.delayAnimate = function (e) {
            var _ = this;
            _.animate(0);
            // 延迟定时器
            clearInterval(_.timer);
            setTimeout(function () {
                _.startUp();
            }, 0);
            e.stopPropagation();
        };

        KaiBanner.prototype.bottomBtnsClickHandle = function (e) {
            var _ = this;
            var i = null;
            var children = _.$bottomBtns.children();
            var length = children.length;

            here: for (i = 0; i < length; i++) {
                if (children[i] == e.target) {
                    _.currentIndex = i;
                    break here;
                }
            }

            if (_.currentIndex === 0) {
                _.direction = 'toRight';
            }
            else if (_.currentIndex == _.maxIndex) {
                _.direction = 'toLeft';
            }
            _.delayAnimate(e);
        };

        KaiBanner.prototype.prevBtnClickHandle = function (e) {
            var _ = this;
            if (_.currentIndex == 1) _.direction = 'toRight';
            if (_.currentIndex === 0) {
                _.currentIndex = _.maxIndex;
                _.direction = 'toLeft';
            }
            else {
                _.currentIndex--;
            }
            _.delayAnimate(e);
        };

        KaiBanner.prototype.nextBtnClickHandle = function (e) {
            var _ = this;
            if (_.currentIndex == _.maxIndex - 1) _.direction = 'toLeft';
            if (_.currentIndex == _.maxIndex) {
                _.currentIndex = 0;
                _.direction = 'toRight';
            }
            else {
                _.currentIndex++;
            }
            _.delayAnimate(e);
        };

        KaiBanner.prototype.initOrdinaryEvents = function (eventsMap) {
            for (var key in eventsMap) {
                var that = this;
                var delegateEventSplitter = /^(\S+)\s*(.*)$/;
                var matchs = key.match(delegateEventSplitter);
                var $node = this.$el.children(matchs[2]);

                // matchs[1]事件类型;matchs[2]选择符
                // $node.length：jQuery中判断节点是否存在
                // 事件委托，所有的事件都委托在 类 上
                if (eventsMap.hasOwnProperty(key) && $node.length > 0) {
                    (function (eventHandleName) {
                        that.$el.on(matchs[1], matchs[2], function (e) {
                            that[eventHandleName](e);
                        });
                    })(eventsMap[key]);
                }
            }
        };

        KaiBanner.prototype.initWindowOnResize = function () {
            var _ = this;
            $(window).on('resize', utils.throttle(function () {
                _.distance = parseInt(_.$el.css("width"));
                _.animate(0);
            }, 50, 1000)
            );
        };

        KaiBanner.prototype.startUp = function () {
            var _ = this;

            //_.distance = parseInt(_.$el.css('width'));
            _.maxIndex = _.$bannerBody.children().length - 1;
            clearInterval(_.timer)
            _.timer = setInterval(function () {
                if (_.direction == 'toRight') {
                    _.currentIndex++;
                    if (_.currentIndex > _.maxIndex) {
                        _.currentIndex = 0;
                        //_.direction = 'toLeft';
                    }
                }
                else if (_.direction == 'toLeft') {
                    _.currentIndex--;
                    if (_.currentIndex <= 0) {
                        _.direction = 'toRight';
                    }
                }
                _.animate(_.config.speed);
            }, _.config.intervalTime);
            //if (_.$bannerBody.length < 2) {
            //    clearInterval(_.timer)
            //}
        };

        KaiBanner.prototype.bindEvent = function () {
            this.config.fixedWidth || this.initWindowOnResize();
            this.initOrdinaryEvents(this.eventsMap);
        };

        KaiBanner.prototype.init = function () {
            this.bindEvent();
            this.startUp();
        };

        return KaiBanner;
    })();

    $.fn.kaiBanner = function (config) {
        return new kaiBanner(this, config);
        //return this.each(function () {
        //    new kaiBanner(this, config);
        //});
    };

})(this, this.jQuery, document);