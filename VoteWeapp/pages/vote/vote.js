// pages/vote/vote.js
const app = getApp()
var votePageTimer = 0
Page({

  /**
   * 页面的初始数据;
   */
  data: {
    title: '',
    disable: false,
    num: 1,
    selected: '',
    voteList: [],
    loaded: false,
    retry: false,
    pageStatus: 'loading' || 'loaded' || 'retry' || 'voted' || 'lostvote',
    tiemoutHanlder: 0,
    timeToStop: 0,
    timeToStopDisplay: '',
    // bgImg: 'https://mioto.milbit.com/voteapp/Goldbackground.png'// app.globalData.bgImgDefault
    bgImg: app.globalData.bgImg
  },

  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.setData({
      bgImg: app.globalData.bgImg//'https://mioto.milbit.com/voteapp/Goldbackground.png'//app.globalData.bgImgDefault
    })
    var self = this
    self.setData({
      timeToStop: Math.floor(-((new Date() - Math.floor(app.globalData.endTimeStamp)) / 1000))
    })
    this.tiemoutHanlder = setTimeout(function () {
      self.setData({
        pageStatus: 'retry'
      })
    }, 20000)
    this.getVoteInfo()
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
  retryLoad: function () {
    this.getVoteInfo()
  },

  getVoteInfo: function () {
    var _this = this;
    this.setData({
      pageStatus: 'loading'
    })
    wx.request({
      url: app.globalData.baseUrl + '/vote/info',
      data: {
      },
      header: {
        'content-type': 'application/json' // 默认值
      },
      success: function (res) {
        clearTimeout(_this.tiemoutHanlder);
        _this.setData({
          timeToStop: _this.data.timeToStop + res.data.CloseSeconds
        })
        if (res.data['voted']) {
          _this.setData({
            pageStatus: 'voted'
          })
          app.globalData.voted = true;
        } else {
          app.globalData.voted = false;
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
                url: '/pages/voteTimeout/voteTimeout',
              })
              // _this.processVoted('null');
            }
          }, 1000)
          _this.setData({
            pageStatus: 'loaded',
            options: res.data.voteList,
            title: res.data.question,
            id: res.data.Id,
            //userId: options.userId
          })
        }
      }
    })
  },
  checkboxChange: function (e) {
    console.log(e.detail)
  },
  showImg: function (e) {
    var src = e.currentTarget.dataset.src//获取data-src
    wx.downloadFile({
      url: src,
      success: function (res) {
        console.log(res)
        wx.previewImage({
          current: res.tempFilePath,
          urls: [res.tempFilePath]// 当前显示图片的http链接
        })
      }
    })

  },
  inArray: function (array, item) {
    for (var i = 0; i < array.length; i++) {
      if (array[i] == item) {
        console.log(i)
        return i
      }
    }
    return -1
  },
  checkboxChange: function (e) {
    this.setData({
      selected: e.detail.value[e.detail.value.length - 1]
    })
  },
  voted: function () {
    var select = this.data.selected//.join(',') 
    var _this = this;
    if (!select) {
      wx.showToast({
        title: '请选择一项投票',
        icon: 'none'
      })
      return;
    }
    wx.showModal({
      title: '请确认',
      content: '确定给他投票吗?',
      success: function (res) {
        if (res.confirm) {
          _this.processVoted(select);
        }
      }
    })
  },
  processVoted: function (select) {
    clearInterval(votePageTimer)
    var VoteId = this.data.id
    wx.request({
      url: app.globalData.baseUrl + '/vote/post', //仅为示例，并非真实的接口地址
      header: {
        'content-type': 'application/json'
      },
      method: "POST",
      data: {
        VoteLog: {
          VoteID: VoteId,
          UserId: app.globalData.openId,
          VoteOption: select,
          Latitude: app.latitude,
          Longtitude: app.longitude
        },
        NickName: app.globalData.userInfo.nickName,
        Avatar: app.globalData.userInfo.avatarUrl
      },
      header: {
        'content-type': 'application/json' // 默认值
      },
      success: function (res) {
        app.globalData.voted = true;
        console.log(res.data)
        if (res.data.msg == 'ok') {
          if (select != 'null')
            wx.redirectTo({
              url: '/pages/thanks/thanks',
            })
        }
      }
    })
  }
})