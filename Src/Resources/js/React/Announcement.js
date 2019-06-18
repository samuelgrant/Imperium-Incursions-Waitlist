import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import Modal from './Components/Modal';
import { TextArea } from './Components/FormControls';
import { XmppLink } from './Components/CommLinks';

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
            console.error(`[React/Announcements@getAnnouncement] Error checking for announcements: ${err.responseText}`)
        });
    }

    hideAnnouncement() {
        $.ajax({
            type: 'post',
            url: `${baseUri}/${this.state.banner.id}/hide`
        }).fail((err) => {
            console.error(`[React/Announcements@hideAnnouncement] Error hiding announcement ${this.state.banner.id}: ${err.responseText}`)
        })
    }

    getIconClass() {
        switch (this.getType()) {
            case 'success':
                return 'fa-check-circle';
            case 'danger':
                return 'fa-engine-warning';
            default:
                return 'fa-info-circle'
        }
    }

    getType() {
        return this.state.banner ? this.state.banner.type : "primary";
    }

    render() {
        let banner;
        if (this.state.banner != null && this.state.display) {
            banner = (
                <Alert type={this.getType()}>
                    <button type="button" className="close" data-dismiss="alert" aria-label="Close" onClick={this.hideAnnouncement.bind(this)}>
                        <span aria-hidden="true">×</span>
                    </button>
                    <div className="row">
                        <div class="col-1 text-center">
                            <i className={`fas ${this.getIconClass()} fa-3x`}></i>
                        </div>

                        <div className="col-11">
                            <h4 className="font-weight-bolder">{this.state.banner.message}</h4>
                            <blockquote className="font-weight-bold text-white">— <XmppLink AuthName={this.state.banner.createdBy} /> {this.state.banner.posted} ago.</blockquote>
                        </div>
                    </div>
                </Alert>
            )
        }

        return (
            <div className="container">
                {banner}
                <AnnouncementModal banner={this.state.banner} u={this.getAnnouncement.bind(this)} />
            </div>
        );
    }
}

export class AnnouncementModal extends Component {
    getType() {
        if (!this.props.banner)
            return { className: "", type: "" };

        return {
            className: this.props.banner.type,
            type: this.props.banner.type.charAt(0).toUpperCase() + this.props.banner.type.slice(1)
        }
    }

    getIssuer() { this.props.banner ? this.props.banner.createdBy : null; }

    timeDiff() { return this.props.banner ? this.props.banner.posted : null; }

    getMessage() { return this.props.banner ? this.props.banner.message : null; }

    deleteAnnouncement() {
        $.ajax({
            type: 'delete',
            url: `${baseUri}/${this.props.banner.id}`
        }).done(() => {
            $(".modal").modal('hide');
            this.props.u();
        }).fail((err) => {
            console.error(`[React/Announcements@deleteAnnouncement] Error hiding announcement ${this.state.banner.id}: ${err.responseText}`)
        })
    }

    createAnnouncement(e) {
        e.preventDefault();

        $.ajax({
            type: 'post',
            url: `${baseUri}`,
            data: {
                type: e.target[0].value,
                message: e.target[1].value
            }
        }).done(() => {
            $(".modal").modal('hide');
            this.props.u();
        }).fail((err) => {
            console.error(`[React/Announcements@createAnnouncement] Error creating announcement ${err.responseText}`)
        });

    }

    render() {
        let modalcontent;
        if (this.props.banner == null) {
            modalcontent = (
                <form onSubmit={this.createAnnouncement.bind(this)}>
                    <div className="form-group">
                        <label htmlFor="AnnouncmentType">Announcement Type:</label>
                        <select className="form-control" name="type">
                            <option value="primary">Information (Blue)</option>
                            <option value="danger">Important (Red)</option>
                            <option value="success">Green</option>
                        </select>
                    </div>

                    <div className="form-group">
                        <TextArea name="message" required="true"/>
                    </div>

                    <button className="btn btn-success" type="submit">Display Announcement <i className="fas fa-check-circle"></i></button>
                </form>
            )
        } else {
            modalcontent = (
                <div>
                    <h4 className={`font-weight-bolder text-${this.getType().className}`} >Announcment Type: {this.getType().type}</h4>
                    <h5 className="font-weight'bold pb-3">Issued by <XmppLink AuthName={this.getIssuer()} /> {this.timeDiff()} ago.</h5>

                    <p className="pb-4">{this.getMessage()}</p>
                    <button className="btn btn-danger" onClick={this.deleteAnnouncement.bind(this)}>Remove Announcement <i className="fas fa-times-circle"></i></button>
                </div>
            );
        }

        return (
            <Modal id="announcmentBannersModal" title="Announcements" dismiss="true">
                {modalcontent}
            </Modal>
        )
    }
}

if (document.getElementById('announcementBanner'))
    render(<AnnouncementBanner />, document.getElementById('announcementBanner'));