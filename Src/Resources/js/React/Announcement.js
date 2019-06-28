import React, { Component } from 'react';
import { render } from 'react-dom';

import Banner from './Components/Announcment/Banner';
import Modal from './Components/Announcment/Modal';

const baseUri = "/api/v1/announcements";

export default class AnnouncementBanner extends Component {
    constructor(props) {
        super(props);
        this.state = { banner: null, display: false }
    }

    componentDidMount() {
        this.getAnnouncement();

        setInterval(() => this.getAnnouncement(), 1000 * 30);
    }

    getAnnouncement() {
        $.ajax({
            type: 'get',
            url: baseUri
        }).done((data, textStatus, xhr) => {
            this.setState({
                banner: data,
                display: xhr.status == 200 ? true : false
            });
        }).fail((err) => {
            this.setState({ display: false });
            console.error(`[React/Announcement] @getAnnouncement - Error retrieving the latest announcment`, err.responseText);
        });
    }

    hide() {
        $.ajax({
            type: 'post',
            url: `${baseUri}/${this.state.banner.id}/hide`
        }).fail((err) => {
            console.error(`[React/Announcement] @hide - Error hiding the announcment`, err.responseText);
        })
    }

    render() {
        let banner;
        if (this.state.banner && this.state.display) {
            banner = (
                <div>
                    <Banner banner={this.state.banner}
                        baseUri={baseUri}
                        hide={this.hide.bind(this)} />
                </div>
            )    
        }

        return (
            <div className="container">
                {banner}

                <Modal banner={this.state.banner}
                    baseUri={baseUri}
                    u={this.getAnnouncement.bind(this)} />
            </div>
        );
    }
}

if (document.getElementById('announcementBanner'))
    render(<AnnouncementBanner />, document.getElementById('announcementBanner'));