// pages/vote/vote.js
var app = getApp()
var votePageTimer = 0
Page({

  /**
   * 页面的初始数据
   */
  data: {
    bgImg: 'https://mioto.milbit.com/voteapp/bgBar.png?v=' + (new Date() - 0),
    Options: [],
    EnableStartTime: 0,
    CloseSeconds: 0,
    ResultDic: {},
    timeToStop: 0
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    var _this = this;
    wx.request({
      url: app.globalData.baseUrl + '/Jdly/GetVote',
      method: 'get',
      success: function (res) {
        var timeToStop = (res.data.EnableStartTime - new Date()) / 1000 + res.data.CloseSeconds
        _this.setData({
          Options: res.data.Options,
          timeToStop: timeToStop,
          JdlyId: res.data.Id
        })
        _this.timer();
        console.log(res)
      }
    })


  },

  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  },
  bindChange: function (e) {
    var _this = this;
    var optIndex = e.currentTarget.dataset.optindex;
    var optValue = e.detail.value;
    var selectValue = _this.data.Options[optIndex]["Option" + (parseInt(optValue) + 1)]
    var resultDicMapping = _this.data.ResultDic;
    resultDicMapping[optIndex] = selectValue;
    this.setData({
      ResultDic: resultDicMapping
    })
  },
  voted: function () {
    var _this = this;
    var rgithNumb = 0;
    var wrongOption = "";
    var optionJson = "";
    var selectCount = 0;
    selectCount = Object.keys(_this.data.ResultDic).length
    if (selectCount != _this.data.Options.length) {
      wx.showToast({
        title: '有未选择项',
        icon: 'none'
      })
      return;
    }
    Object.keys(_this.data.ResultDic).forEach(function (item) {
      optionJson += item + ":" + _this.data.ResultDic[item] + "|"
      if (_this.data.ResultDic[item] == _this.data.Options[item].Answer) {
        rgithNumb++
      } else {
        wrongOption += item + ":" + _this.data.ResultDic[item] + "|"
      }
    })
    wrongOption.substring(0, wrongOption.length - 1)
    optionJson.substring(0, optionJson.length - 1)
    var postData = {
      JdlyId: _this.data.JdlyId,
      UserCode: app.globalData.userCode,
      RightNumber: rgithNumb,
      WrongOption: wrongOption,
      OptionJson: optionJson,
      NickName: app.globalData.userInfo.nickName,
      Avarta: app.globalData.userInfo.avatarUrl
    }
    wx.request({
      url: app.globalData.baseUrl + '/jdly/Post',
      data: postData,
      success: function (res) {
        if (res.data.success) {
          wx.redirectTo({
            url: '/pages/thanks/thanks'
          })
        }
      }
    })
  },
  timer: function () {
    var _this = this;
    votePageTimer = setInterval(function () {
      if (_this.data.timeToStop > 0) {
        var t = _this.data.timeToStop * 1000
        var seconds = Math.floor((t / 1000) % 60)
        var minutes = Math.floor((t / 1000 / 60) % 60)
        var hours = Math.floor((t / (1000 * 60 * 60)) % 24)
        var days = Math.floor(t / (1000 * 60 * 60 * 24))
        var displayValue = '';
        if (days > 0) {
          displayValue += days + ' days : ';
        }
        if (hours > 0) {
          displayValue += hours + ' hours : ';
        }
        if (minutes > 0) {
          displayValue += minutes + ' min : ';
        }
        if (seconds) {
          displayValue += seconds + ' sec : ';
        }
        _this.setData({
          timeToStop: _this.data.timeToStop - 1,
          timeToStopDisplay: minutes + " min : " + seconds + " sec"
        })
      }
      else {
        // _this.setData({
        //   pageStatus: 'lostvote'
        // })
        clearInterval(votePageTimer)
        wx.redirectTo({
          url: '/pages/thanks/thanks',
        })
        // _this.processVoted('null');
      }
    }, 1000)
  }
})