import React, { Component } from 'react';
import Alert from '../Alert';
import { XmppLink } from '../CommLinks';


export default class Banner extends Component {
    getType() {
        return this.props.banner ? this.props.banner.type : "primary";
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

    render() {
        return (
            <Alert type = { this.getType() } >
                <button type="button" className="close" data-dismiss="alert" aria-label="Close" onClick={this.props.hide}>
                    <span aria-hidden="true">×</span>
                </button>
                <div className="row">
                    <div class="col-1 text-center">
                        <i className={`fas ${this.getIconClass()} fa-3x`}></i>
                    </div>

                    <div className="col-11">
                        <h4 className="font-weight-bolder">{this.props.banner.message}</h4>
                        <blockquote className="font-weight-bold text-white">— <XmppLink AuthName={this.props.banner.createdBy} /> {this.props.banner.posted} ago.</blockquote>
                    </div>
                </div>
            </Alert>
        )
    }
}