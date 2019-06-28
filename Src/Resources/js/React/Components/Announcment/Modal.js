import React, { Component } from 'react';

import Modal from '../Modal';
import { TextArea } from '../FormControls';
import { XmppLink } from '../CommLinks';


export default class BannerModal extends Component {
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
            url: `${this.props.baseUri}/${this.props.banner.id}`
        }).done(() => {
            $(".modal").modal('hide');
            this.props.u();
        }).fail((err) => {
            console.error(`[React/Modal (Announcement)] @deleteAnnouncement - Error deleting announcment`, err.responseText);
        })
    }

    createAnnouncement(e) {
        e.preventDefault();

        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}`,
            data: {
                type: e.target[0].value,
                message: e.target[1].value
            }
        }).done(() => {
            $(".modal").modal('hide');
            this.props.u();
        }).fail((err) => {
            console.error(`[React/Modal (Announcement)] @createAnnouncement - Error creating a new announcment`, err.responseText);
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
                        <TextArea name="message" required="true" />
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