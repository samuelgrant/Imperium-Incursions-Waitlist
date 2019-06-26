import React, { Component } from 'react';
import { TextArea, Input } from '../FormControls'
import { DateFormat, AccountPilot } from '../../Helpers';


export default class BanInfo extends Component {
    // The next three methods are needed to make autocomplete work when the component is controlled by a key
    componentDidUpdate() { this.configureAutocomplete() }
    componentDidMount() { this.configureAutocomplete() }

    configureAutocomplete() {
        $("#lookup_account").autocomplete({
            source: (request, response) => {
                $.ajax({
                    url: `/search?q=${request.term}&filter=account`,
                    dataType: "json",
                    success: function (data) {
                        response(data);
                    }
                });
            },
            minLength: 3,
            delay: 500
        })
    }

    inputNewBan() {
        return this.props.selectedBan == null;
    }

    getPilotUrl() {
        let pilot_id = 0;
        if (this.props.selectedBan && this.props.selectedBan.bannedAccount.pilots[0])
            pilot_id = AccountPilot(this.props.selectedBan.bannedAccount.name, this.props.selectedBan.bannedAccount.pilots).id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_128.jpg`;
    }

    newBan(e) {
        e.preventDefault();
        
        $.ajax({
            type: 'POST',
            url: this.props.baseUri,
            data: $("#banForm").serialize()
        }).done((data) => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/BanInfo] @newBan - Error creating a ban`, err.responseText);
        });
    }

    updateBan(e) {
        e.preventDefault();
        
        $.ajax({
            type: 'put',
            url: `${this.props.baseUri}/${this.props.selectedBan.id}`,
            data: $("#banForm").serialize()
        }).done((data) => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/BanInfo] @updateBan - Error updating ban (ban Id: ${this.props.selectedBan.id})`, err.responseText);
        });
    }

   
    render() {
        let banIssuedAt;
        if (!this.inputNewBan()) {
            banIssuedAt = (
                <div className="form-group">
                    <label htmlFor="createdAt">Ban issued:</label>
                    <Input id="createdAt" type="text" value={DateFormat(this.props.selectedBan.createdAt)} disabled="true" key={this.props.selectedBan.id} />
                </div>
            )
        }

        // Reset buttons
        let reset_btn = {};
        if (!this.inputNewBan()) {
            reset_btn.large = <button className="btn btn-dark float-left" type="button" onClick={this.props.setIndex.bind(this, null)}>Back <i className="fas fa-undo-alt"></i></button>;
            reset_btn.small = <i className="fas fa-times-circle float-right mr-3 i-btn" onClick={this.props.setIndex.bind(this, null)}></i>
        }

        let button;
        if (this.props.admin && this.inputNewBan()) {
            button = <button className="btn btn-danger float-right" type="submit">Ban User <i className="fas fa-gavel"></i></button>
        } else if(this.props.admin) {
            button = <button className="btn btn-danger float-right" type="submit">Update Ban <i className="fas fa-gavel"></i></button>
        }
        
        return (
            <div>
                <div className="panel-body py-4">
                    {reset_btn.small}
                    <img className="rounded-circle d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" />

                    <form id="banForm" onSubmit={this.inputNewBan() ? this.newBan.bind(this) : this.updateBan.bind(this)}>
                        <div className="form-group">
                            <label htmlFor="#lookup_account">GSF Auth Name:</label>
                            <Input id="lookup_account"
                                type="text"
                                classOverride="form-control account_lookup"
                                name="accountName"
                                value={this.props.selectedBan ? this.props.selectedBan.bannedAccount.name : ""}
                                disabled={this.props.selectedBan ? "true" : "false"}
                                key={this.props.selectedBan ? this.props.selectedBan.id : 0}
                                required="true"
                                autocomplete="off" />
                        </div>

                        {banIssuedAt}

                        <div className="form-group">
                            <label htmlFor="banExpires">Ban Expires:</label>
                            <Input type="text" id="banExpires" disabled="true" />
                            <small className="text-muted">Bans expire at downtime, leave blank for permanant</small>
                        </div>

                        <div className="form-group">
                            <label htmlFor="banReason">Reason:</label>
                            <TextArea id="banReason"
                                name="banReason"
                                value={this.props.selectedBan ? this.props.selectedBan.reason : ""}
                                key={this.props.selectedBan ? this.props.selectedBan.id : 0}
                                required="true" />
                            <small className="text-muted">Only visible to the FC team</small>
                        </div>

                        {reset_btn.large}
                        {button}
                    </form>
                </div>
            </div>
        )
    }
}